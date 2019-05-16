#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Models
{
	using Core;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public class RouteViewModel
	{
		[JsonProperty("type")]
		[JsonConverter(typeof(StringEnumConverter))]
		public RouteType Type { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("symbol")]
		public string Symbol { get; set; }

		[JsonProperty("driver")]
		public string Driver { get; set; }

		[JsonProperty("status")]
		[JsonConverter(typeof(StringEnumConverter))]
		public RouteStatus Status { get; set; }

		[JsonProperty("count")]
		public int Stops { get; set; }

		public static implicit operator RouteViewModel(RouteManifest route)
		{
			return new RouteViewModel
			{
				Name = route.Name,
				Status = route.Status,
				Symbol = route.Symbol,
				Type = route.Type,
				Stops = route.Stops.Count,
				Driver = route.Driver
			};
		}
	}
}