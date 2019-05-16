#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Core
{
	using System.Collections.Concurrent;

    public class Region
	{
		public string Name { get; set; }
		public string Symbol { get; set; }
		public Address Hub { get; set; }

		public ConcurrentDictionary<string, RouteManifest> Routes { get; } = new ConcurrentDictionary<string, RouteManifest>();

		public override string ToString()
		{
			return $"{Symbol}: {Name}: {Routes.Count} routes.";
		}
	}
}
