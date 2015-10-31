using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace GraduationProject
{
	class Equation
	{
		public bool IsEquation = false;
		public bool IsLinear = true;

		public Factor Result;

		public string PrintResult ()
		{
			string eq = "";
			foreach (var f in Result.Terms) {
				eq += f.Coefficient + " * " + f.VariablesToString () + "\n";
			}
			return eq;
		}
	}
}
