using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProject
{
	/// <summary>
	/// Represents a methematical factor.
	/// eg (2x^2 + 3y).
	/// Finally the whole equation should be a single factor.
	/// </summary>
	class Factor
	{
		/// <summary>
		/// List of terms.
		/// {2, x^2}, {4}, {-3, y} => 2x^2 +4 -3y
		/// </summary>
		public List<Term> Terms = new List<Term> ();
        
		//Constructors.
		public Factor ()
		{
		}

		public Factor (Term term)
		{
			Terms.Add (term);
		}

		public Factor (Factor factor)
		{
			Terms = factor.Terms;
		}

		/// <summary>
		/// Adds a term.
		/// eg. {2, x^2}, {4} + {-3, y}
		/// </summary>
		/// <param name="f">the term to be added.</param>
		public void Add (Term term)
		{
			foreach (var t in Terms) {
				if (t.VariablesToString ().Equals (term.VariablesToString ())) {
					t.Coefficient += term.Coefficient;
					return;
				}
			}
			Terms.Add (term);
		}

		/// <summary>
		/// Substracts a term.
		/// eg. {2, x^2}, {4} - {-3, y}
		/// </summary>
		/// <param name="term">The term to be substracted.</param>
		public void Subtract (Term term)
		{
			term.Coefficient *= -1;
			Add (term);
		}

		/// <summary>
		/// Adds a factor.
		/// eg. {2, x^2}, {4} + {-3, y}, {2}
		/// </summary>
		/// <param name="factor"></param>
		public void Add (Factor factor)
		{
			Factor rFactor = this;
			Factor lFactor = factor;
			Factor result = new Factor ();

			if (lFactor.Terms.Count > 0)
				for (int i = lFactor.Terms.Count - 1; i >= 0; i--) {
					bool done = false;
                
					if (rFactor.Terms.Count > 0)
						for (int j = rFactor.Terms.Count - 1; j >= 0; j--) {
							if (lFactor.Terms [i].VariablesToString ().Equals (rFactor.Terms [j].VariablesToString ())) {
								lFactor.Terms [i].Coefficient += rFactor.Terms [j].Coefficient;
								if (lFactor.Terms [i].Coefficient != 0)
									result.Add (lFactor.Terms [i]);
								lFactor.Terms.RemoveAt (i);
								rFactor.Terms.RemoveAt (j);
								done = true;
								break;
							}
						}

					if (!done) {
						result.Add (lFactor.Terms [i]);
						lFactor.Terms.RemoveAt (i);
					}
				}

			if (rFactor.Terms.Count > 0)
				foreach (var rTerm in rFactor.Terms) {
					result.Add (rTerm);
				}

			this.Terms = result.Terms;
		}

		/// <summary>
		/// Subtracts a factor.
		/// eg. {2, x^2}, {4} - {-3, y}, {2}
		/// </summary>
		/// <param name="factor"></param>
		public void Subtract (Factor factor)
		{
			foreach (var f in factor.Terms)
				f.Coefficient *= -1;

			Add (factor);
		}

		/// <summary>
		/// Multiply a Factor by a Term.
		/// eg. (2x) * (2x + 3y)
		/// </summary>
		/// <param name="term">The term to be multiplied.</param>
		public void Multiply (Term term)
		{
			foreach (var factor in Terms) {
				factor.Multiply (term);
			}
		}

		/// <summary>
		/// Divide by a term.
		/// eg 2x / (2x + 1)
		/// </summary>
		/// <param name="term">The term to be devided by.</param>
		public void Divide (Term term)
		{
			Term temp;
			foreach (var factor in Terms) {
				temp = term.Clone ();
				temp.Divide (factor);
				factor.Coefficient = temp.Coefficient;
				factor.Variables = temp.Variables;
			}
		}

		/// <summary>
		/// Multiply a factor by a factor.
		/// (2x + 1) * (x + 3)
		/// </summary>
		/// <param name="factor"></param>
		public void Multiply (Factor factor)
		{
			Factor result = new Factor ();
			Factor rTerm;

			foreach (var f in Terms) {
				rTerm = factor.Clone ();
				rTerm.Multiply (f);
				result.Add (rTerm);
			}

			Terms = result.Terms;
		}

		/// <summary>
		/// Divide by a factor.
		/// eg. (2x + 1) / (3x + 2)
		/// </summary>
		/// <param name="t">The factor to be devided by.</param>
		public void Divide (Factor t)
		{
			Factor result = new Factor ();
			Factor rTerm;

			foreach (var factor in Terms) {
				rTerm = t.Clone ();
				rTerm.Divide (factor);
				result.Add (rTerm);
			}

			Terms = result.Terms;
		}

		public Factor Clone ()
		{
			Factor t = new Factor ();
			foreach (var factor in Terms) {
				t.Terms.Add (factor.Clone ());
			}
			return t;
		}
	}
}
