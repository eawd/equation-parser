using System;
using System.Text;

namespace GraduationProject
{
	public class ParserTester
	{
		Parser parser;

		public ParserTester (string equation)
		{
			parser = new Parser (equation);
		}

		public string Result {
			get {
				if (parser.Result.Terms.Count == 0)
					return "0";

				return string.Join (" + ", parser.Result.Terms) + ((parser.IsEquation) ? " = 0" : "");
			}
		}
	}
}

