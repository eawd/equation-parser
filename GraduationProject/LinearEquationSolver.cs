/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace GraduationProject.Solver
{
	public class LinearEquationSolver
	{
		List<Term> results = new List<Term> ();
		Dictionary<string, int> indexes = new Dictionary<string, int> ();
		int currentIndex = 0;

		public LinearEquationSolver (List<string> equations)
		{
			foreach (string equation in equations) {
				Parser p = new Parser (equation);
				if (p.eq.IsLinear) {
					results.Add (p.eq.Result);
				} else {
					throw new Exception (String.Format ("Equation {0} is not linear.", equation));
				}
			}
		}

		int getIndex (string variableName)
		{
			if (indexes.ContainsKey (variableName))
				return indexes [variableName];
			else {
				indexes.Add (variableName, currentIndex);
				currentIndex++;
				return currentIndex - 1;
			}

		}

		Tuple<Matrix<double>, Vector<double>> populateMatrix ()
		{
			List<Vector<double>> l = new List<Vector<double>> ();
			List<double> motlaq = new List<double> ();

			foreach (var result in results)
				foreach (var f in result.Factors)
					if (!String.IsNullOrEmpty (f.VariablesToString ()))
						getIndex (f.VariablesToString ());

			foreach (var result in results) {
				int equationNumber = 0;
				List<double> v = new List<double> ();
				foreach (var f in result.Factors) {
					if (String.IsNullOrEmpty (f.VariablesToString ())) {
						resize (motlaq, equationNumber);
						motlaq.Insert (equationNumber, (double)f.Coefficient * -1);
					} else {
						int index = getIndex (f.VariablesToString ());
						if (v.Count < index)
							resize (v, index);
						v.Insert (index, (double)f.Coefficient);
					}

					equationNumber++;
				}
				Vector<double> d = Vector<double>.Build.DenseOfArray (v.ToArray ());
				l.Add (d);
			}

			Matrix<double> m = Matrix<double>.Build.DenseOfRowVectors (l);
			Vector<double> t = Vector<double>.Build.DenseOfArray (motlaq.ToArray ());
			return new Tuple<Matrix<double>, Vector<double>> (m, t);
		}

		public Matrix<double> GetMatrix ()
		{
			return populateMatrix ().Item1;
		}

		Vector<double> solve ()
		{
			Tuple<Matrix<double>, Vector<double>> t = populateMatrix ();
			return t.Item1.Solve (t.Item2);
		}

		public Dictionary<string, double> GetResults ()
		{
			var result = new Dictionary<string, double> ();
			var solution = solve ();
			foreach (var i in indexes) {
				result.Add (i.Key, solution.ElementAtOrDefault (i.Value));
			}
			return result;
		}

		void resize (List<int> list, int to)
		{
			int count = to - list.Count + 1;
			for (int i = 0; i < count; i++) {
				list.Add (0);
			}
		}

		void resize (List<double> list, int to)
		{
			int count = to - list.Count;
			for (int i = 0; i < count; i++) {
				list.Add (0);
			}
		}
	}
}

//*/