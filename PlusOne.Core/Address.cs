#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Core
{
	using System.Linq;

	public class Address
	{
		public string[] Street { get; set; }
		public string City { get; set; }
		public string Postal { get; set; }
		public string State { get; set; }
		public bool IsResidence { get; set; }

		public override string ToString()
		{
			return $"{Street.First()} {City}, {State} {Postal}";
		}
	}
}