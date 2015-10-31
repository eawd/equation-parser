using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace GraduationProject
{
	/// <summary>
	/// Represents a mathematical term eg. (2xy), (2), (2x^2)
	/// </summary>
	class Term
	{
		decimal coefficient = 1;
		public SortedDictionary<string, decimal> Variables = new SortedDictionary<string, decimal> ();

		public Term ()
		{
		}

		public Term (decimal coefficient)
		{
			Coefficient = coefficient;
		}

		public Term (string variable, decimal power)
		{
			AddVariable (variable, power);
		}

		public Term (Term f)
		{
			Multiply (f);
		}

		/// <summary>
		/// Multiplies by variable or adds it to variable list if it doesn't exist.
		/// eg (Term * x^2) or (Term * x)
		/// </summary>
		/// <param name="variable">Variable name.</param>
		/// <param name="power">The power it's raised to.</param>
		public void AddVariable (string variable, decimal power = 1)
		{
			if (Variables.ContainsKey (variable)) {
				Variables [variable] += power;
				if (Variables [variable] == 0)
					Variables.Remove (variable);
				return;
			}
			Variables.Add (variable, power);
		}

		/// <summary>
		/// Multiplies to Terms and adds the result to this Term.
		/// eg (2xy * 3x^2).
		/// </summary>
		/// <param name="f">The term to be multiplied.</param>
		public void Multiply (Term f)
		{
			Coefficient *= f.Coefficient;
			if (Coefficient == 0) {
				Variables = new SortedDictionary<string, decimal> ();
				return;
			}
			foreach (var variable in f.Variables)
				AddVariable (variable.Key, variable.Value);
		}

		/// <summary>
		/// Divides by a Term result is stored in current term.
		/// eg (2x / 3y)
		/// </summary>
		/// <param name="f">The term to be devided.</param>
		public void Divide (Term f)
		{
			Coefficient /= f.Coefficient;
			if (Coefficient == 0) {
				Variables = new SortedDictionary<string, decimal> ();
				return;
			}
			foreach (var variable in f.Variables)
				AddVariable (variable.Key, variable.Value * -1);
		}

		/// <summary>
		/// The coefficient of the term.
		/// eg. the 2 in 2x^2.
		/// </summary>
		public decimal Coefficient {
			get { return coefficient; }
			set { coefficient = value; }
		}

		/// <summary>
		/// Converts the list of variables to string representation.
		/// </summary>
		/// <returns>A string representation of variables eg. (x^2 * y^2)</returns>
		public string VariablesToString ()
		{
			if (Variables.Any ())
				return string.Join (" * ", Variables.Select (kv => kv.Key.ToString () + "^" + kv.Value.ToString ()).ToArray ());
			else
				return String.Empty;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="GraduationProject.Term"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="GraduationProject.Term"/>.</returns>
		public override string ToString ()
		{
			if (Variables.Count > 0)
				return string.Format ("({0} * {1})", Coefficient, VariablesToString ());
			else
				return string.Format ("{0}", Coefficient);
		}

		public Term Clone ()
		{
			Term f = new Term ();
			f.coefficient = coefficient;
			foreach (var variable in Variables) {
				f.Variables.Add (variable.Key, variable.Value);
			}
			return f;
		}
	}
}
