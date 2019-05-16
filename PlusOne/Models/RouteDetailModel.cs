#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Models
{
	using Core;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public class RouteDetailModel
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("type")]
		[JsonConverter(typeof(StringEnumConverter))]
		public StopType Type { get; set; }

		[JsonProperty("order")]
		public int Order { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("address")]
		public string StreetCityPostal { get; set; }

		public static implicit operator RouteDetailModel(Stop stop)
		{
			return new RouteDetailModel
			{
				Id = stop.Id,
				Type = stop.Type,
				Order = stop.Order,
				Name = stop.Name ?? "Name on Package",
				StreetCityPostal = stop.Address.ToString(),
			};
		}
	}
}