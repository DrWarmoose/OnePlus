#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Core
{
	using System;
	using System.Collections.Concurrent;

	public class RouteManifest
	{
		public int Proximity { get; set; }
		public RouteType Type { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public string Driver { get; set; }
		public RouteStatus Status { get; set; }
		public Visit Current { get; set; }
		public ConcurrentDictionary<string, Stop> Stops { get; } = new ConcurrentDictionary<string, Stop>();
	}
}