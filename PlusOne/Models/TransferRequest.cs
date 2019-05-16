#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Models
{
	using Newtonsoft.Json;

	public class TransferRequest
	{
		[JsonProperty("stop")]
		public string StopId { get; set; }

		[JsonProperty("from")]
		public string FromRouteId { get; set; }

		[JsonProperty("to")]
		public string ToRouteId { get; set; }
	}
}