using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GraduationProject
{
	/// <summary>
	/// Parser class, takes an equation and returns a list of variable, coefficient pairs.
	/// </summary>
	class Parser
	{
		Token lookahead;
		Equation eq = new Equation ();

		public Factor Result {
			get { return eq.Result; }
		}

		public bool IsEquation {
			get { return eq.IsEquation; }
		}

		Lexer lexer;

		public Parser (string equation)
		{
			if (Regex.Match (equation, @"\=").Success)
				eq.IsEquation = true;
			if (Regex.Match (equation, @"[a-zA-Z]+\^[2-9][0-9]+").Success)
				eq.IsLinear = false;

			equation = Regex.Replace (equation, @"([0-9]+)\s*([a-zA-Z]+)", "$1*$2");
			equation = Regex.Replace (equation, @"([a-zA-Z]+)\s*([0-9]+)", "$1^$2");
			equation = Regex.Replace (equation, @"([a-zA-Z0-9\)]+)\s*(\()", "$1*$2");

			MemoryStream stream = new MemoryStream ();
			StreamWriter sw = new StreamWriter (stream);
			sw.Write (equation);
			sw.Flush ();
			stream.Position = 0;
			lexer = new Lexer (stream);

			eq.Result = Parse ();
		}

		Exception WrongSymbol (string expected, string found)
		{
			return new Exception (String.Format ("Expected '{0}' but found '{1}'", expected, found));
		}

		void GetNextToken ()
		{
			lookahead = lexer.getNextToken ();
			if (lookahead == null)
				lookahead = new Token (TokenType.EOF, "End of file", 0, 0);
		}

		Factor Parse ()
		{
			GetNextToken ();
			Factor t1 = Expression ();
			if (!eq.IsEquation && lookahead.type == TokenType.EOF)
				return t1;
			if (!eq.IsEquation && lookahead.type != TokenType.EOF)
				throw WrongSymbol ("End of File", lookahead.text);
			if (eq.IsEquation && lookahead.type != TokenType.EQUAL)
				throw WrongSymbol ("=", lookahead.text);
			GetNextToken ();
			Factor t2 = Expression ();
			t1.Subtract (t2);
			return t1;
		}

		//Expr. -> SignedTerm SumOp
		Factor Expression ()
		{
			Factor t = SignedTerm ();
			return SumOp (t);
		}

		//SumOp -> + Term SumOp
		//SumOp -> - Term SumOp
		//SumOp -> EPSILON
		Factor SumOp (Factor t)
		{
			Factor t2;
			switch (lookahead.type) {
			case TokenType.ADD:
				GetNextToken ();
				t2 = Term ();
				t.Add (t2);
				return SumOp (t);
			case TokenType.SUB:
				GetNextToken ();
				t2 = Term ();
				t.Subtract (t2);
				return SumOp (t);
			default:
				return t;
			}
		}
		//SignedTerm -> + Term | - Term | Term
		Factor SignedTerm ()
		{
			switch (lookahead.type) {
			case TokenType.ADD:
				GetNextToken ();
				return Term ();
			case TokenType.SUB:
				GetNextToken ();
				Factor t = Term ();
				t.Multiply (new Term (-1));
				return t;
			default:
				return Term ();
			}
		}

		//Term -> Factor TermOp
		Factor Term ()
		{
			Factor f = Factor ();
			return TermOp (f);
		}

		// TermOp -> * SignedFactor TermOp
		// TermOp -> / SignedFactor TermOp
		// TermOp -> EPSILON
		Factor TermOp (Factor factor)
		{
			switch (lookahead.type) {
			case TokenType.MULTI:
				GetNextToken ();
				factor.Multiply (SignedFactor ());
				return TermOp (factor);
			case TokenType.DEV:
				GetNextToken ();
				factor.Divide (SignedFactor ());
				return TermOp (factor);
			default:
				return factor;
			}
		}

		//SignedFactor -> + Factor | - Factor | Factor
		Factor SignedFactor ()
		{
			switch (lookahead.type) {
			case TokenType.ADD:
				GetNextToken ();
				return Factor ();
			case TokenType.SUB:
				GetNextToken ();
				Factor t = Factor ();
				t.Multiply (new Term (-1));
				return t;
			default:
				return Factor ();
			}
		}

		//Factor -> Value | ( Expression );
		Factor Factor ()
		{
			switch (lookahead.type) {
			case TokenType.COS:
			case TokenType.COSH:
			case TokenType.LN:
			case TokenType.LOG:
			case TokenType.ROOT:
			case TokenType.SQRT:
			case TokenType.SIN:
			case TokenType.SINH:
			case TokenType.TAN:
			case TokenType.TANH:
				return Value ();
			case TokenType.NUM:
				return Value ();
			case TokenType.VAR:
				return Value ();
			case TokenType.LPARENT:
				GetNextToken ();
				Factor t = Expression ();
				if (lookahead.type == TokenType.RPARENT) {
					GetNextToken ();
					return t;
				} else {
					throw WrongSymbol (")", lookahead.text);
				}
			default:
				throw WrongSymbol ("Number, Variable or \"(\"", lookahead.text);
                    
			}
		}

		//Value -> num | num ^ num
		//Value -> var | var ^ num
		//Value -> ${FUNCTION} NUM | ${FUNCTION} ( NUM )
		Factor Value ()
		{
			double number = 1;
			switch (lookahead.type) {
			//Value -> num | num ^ num
			case TokenType.NUM:
				number = double.Parse (lookahead.text);
				GetNextToken ();
				if (lookahead.type == TokenType.POW) {
					GetNextToken ();
					if (lookahead.type != TokenType.NUM)
						throw WrongSymbol ("Number", lookahead.text);
					number = Math.Pow (number, double.Parse (lookahead.text));
					GetNextToken ();
				}
				return new Factor (new Term ((decimal)number));

			//Value -> var | var ^ num
			case TokenType.VAR:
				string var = lookahead.text;
				decimal power = 1;
				GetNextToken ();
				if (lookahead.type == TokenType.POW) {
					GetNextToken ();
					if (lookahead.type != TokenType.NUM)
						throw WrongSymbol ("Number", lookahead.text);
					power = decimal.Parse (lookahead.text);
					GetNextToken ();
				}
				return new Factor (new Term (var, power));

			//Value -> ${FUNCTION} NUM | ${FUNCTION} ( NUM )
			default:
				TokenType op = lookahead.type;
				bool parentOpen = false;
				GetNextToken ();
				if (lookahead.type == TokenType.LPARENT) {
					GetNextToken ();
					parentOpen = false;
				}
				if (lookahead.type != TokenType.NUM) {
					throw WrongSymbol ("Number", lookahead.text);
				}
				number = double.Parse (lookahead.text);
				GetNextToken ();
				if (parentOpen && lookahead.type == TokenType.RPARENT) {
					GetNextToken ();
				}
				if (parentOpen && lookahead.type != TokenType.RPARENT) {
					throw WrongSymbol (")", lookahead.text);
				}
				return DoMathOperation (op, number);
			}
		}

		/// <summary>
		/// Execute Mathematical Function eg. Sin 10 or Sin(10)
		/// </summary>
		/// <param name="operation">TokenType of the operation</param>
		/// <param name="number">operation parameter.</param>
		/// <returns></returns>
		Factor DoMathOperation (TokenType operation, double number)
		{
			double result = 0;
			switch (operation) {
			case TokenType.COS:
				result = Math.Cos (number);
				break;
			case TokenType.COSH:
				result = Math.Cosh (number);
				break;
			case TokenType.LN:
				result = Math.Log (number);
				break;
			case TokenType.LOG:
				result = Math.Log10 (number);
				break;
			case TokenType.ROOT:
				result = Math.Sqrt (number);
				break;
			case TokenType.SQRT:
				result = Math.Sqrt (number);
				break;
			case TokenType.SIN:
				result = Math.Sin (number);
				break;
			case TokenType.SINH:
				result = Math.Sinh (number);
				break;
			case TokenType.TAN:
				result = Math.Tan (number);
				break;
			case TokenType.TANH:
				result = Math.Tanh (number);
				break;
			}
			return new Factor (new Term ((decimal)result));
		}
	}
}
