#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Models
{
	using System.Linq;
	using System.Web;
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

		[JsonProperty("airBills")]
		public string Airbills { get; set; }

		[JsonProperty("service")]
		[JsonConverter(typeof(StringEnumConverter))]
		public ServiceLevel Service { get; set; }

		[JsonProperty("signature")]
		[JsonConverter(typeof(StringEnumConverter))]
		public SignatureRequired Signature { get; set; }

		[JsonProperty("bing")]
		public string BingSearch { get; set; }

		public static implicit operator RouteDetailModel(Stop stop)
		{
			var encCity = HttpUtility.UrlEncode(stop.Address.City);
			var encStreet = HttpUtility.UrlEncode(stop.Address.Street.FirstOrDefault());
			var encoded = $"/US/TX/{stop.Address.Postal}/{encCity}/{encStreet}";

			return new RouteDetailModel
			{
				Id = stop.Id,
				Type = stop.Type,
				Order = stop.Order,
				Name = stop.Name ?? "Name on Package",
				StreetCityPostal = stop.Address.ToString(),
				Airbills = string.Join(", ",stop.Packages.Keys),
				Service =  stop.ServiceLevel,
				Signature = stop.Packages.Any() ? stop.Packages.Values.Max(x => x.Signature) : SignatureRequired.None,
				BingSearch = encoded
			};
		}
	}
}