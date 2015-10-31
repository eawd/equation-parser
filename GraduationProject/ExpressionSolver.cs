using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationProject.Solver
{
	public class ExpressionSolver
	{
		Factor result;

		public decimal Result {
			get { return result.Terms [0].Coefficient; }
		}

		public ExpressionSolver (string expression)
		{
			string temp = Regex.Replace (expression, @"(sin|cos|tan|cosh|sinh|tanh|root|sqrt|ln|log)", "", RegexOptions.IgnoreCase);
			if (Regex.Match (temp, @"[a-zA-Z]+").Success)
				throw new Exception ("Invalid Expression.");
			if (Regex.Match (temp, "=").Success)
				throw new Exception ("Expected an Expression found an Equation.");
			
			Parser p = new Parser (expression);
			result = p.Result;
		}
	}
}
