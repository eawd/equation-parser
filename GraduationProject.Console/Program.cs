using System;
using GraduationProject;

namespace GraduationProject.Console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			System.Console.WriteLine (@"This is a mathematical equations parser and simplifier.
In this demo you can, expand and simplify a mathematical equation or expression, or evaluate an expression.");
			for (;;) {
				RunDemo ();
			}
		}

		private static void RunDemo ()
		{
			System.Console.WriteLine ("\n~~~~~~~~~~~~~~~~~~~~~~");
			System.Console.WriteLine ("Enter 1 to evaluate an expression or 2 to parse an equation or expression.");

			string choice = System.Console.ReadLine ();
			int c = 0;
			try {
				c = int.Parse (choice);
			} catch {
				System.Console.Error.WriteLine ("\"{0}\" is an invalid choice, please try again.", choice);
				return;
			}

			try {
				switch (c) {
				case 1:
					ExpressionSolverTest ();
					break;
				case 2:
					ParserTest ();
					break;
				default:
					System.Console.Error.WriteLine ("{0} is an invalid choice, try again.", c);
					break;
				}
			} catch (Exception e) {
				System.Console.Error.WriteLine ("Error: {0}", e.Message);
			}
		}

		private static void ParserTest ()
		{
			System.Console.WriteLine ("Enter an equation to parse: ");
			string equation = System.Console.ReadLine ();
			ParserTester pt = new ParserTester (equation);
			System.Console.Write ("Result: \n{0}", pt.Result);
		}

		private static void ExpressionSolverTest ()
		{
			System.Console.WriteLine ("Enter an expression to solve: ");
			string exp = System.Console.ReadLine ();
			var s = new Solver.ExpressionSolver (exp);
			System.Console.Write ("Result = {0}", s.Result);
		}
	}
}
