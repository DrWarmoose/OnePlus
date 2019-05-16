namespace PlusOne.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Http;
	using System.Web.Http;
	using Core;
	using Generator;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public class RouteController : ApiController
    {
		RouteManager _manager = new RouteManager();

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

		public class TransferRequest
		{
			[JsonProperty("stop")]
			public string StopId { get; set; }

			[JsonProperty("from")]
			public string FromRouteId { get; set; }

			[JsonProperty("to")]
			public string ToRouteId { get; set; }
		}

		public class RouteManager
	    {
		    private static Region _region;
		    private static FleetSimulator _sim;

		    public RouteManager()
		    {
			    if (_region == null)
				    _region = RegionGenerator.Create("San Antonio", "STX");
			    //if (_sim == null)
				  //  _sim = new FleetSimulator(_region);
		    }

		    public Region GetRegion()
		    {
			    return _region;
		    }

		    public RouteDetailModel[] GetRouteDetail(string symbol)
		    {
			    if (_region.Routes.TryGetValue(symbol, out var manifest))
			    {
				    return manifest.Stops.Values.OrderBy(x => x.Order)
					    .ThenBy(y => y.Id)
					    .Select(map => (RouteDetailModel) map).ToArray();
			    }
				return null;
			}

		    public Dictionary<string, RouteManifest> GetRouteList()
		    {
				var mapping = new Dictionary<string,RouteManifest>();
				foreach( var map in _region.Routes )
					mapping.Add(map.Key,map.Value);

				return mapping;
		    }

		    public RouteDetailModel[] AssignOnCall(string stop, string toRoute)
		    {
			    //_sim.AssignOnCall(oncall, route);
			    //return _region;

				var route = _region.Routes.Values.FirstOrDefault(x => x.Stops.ContainsKey(stop));
				if (route != null && 
					_region.Routes.TryGetValue(toRoute, out var transfer) &&
				    route.Stops.TryGetValue(stop,out var existing))
				{
					existing.Type = StopType.Pickup;
					existing.Order = Convert.ToInt32(DateTime.UtcNow.Ticks % Convert.ToInt64(int.MaxValue));
					transfer.Stops.TryAdd(existing.Id, existing);
					route.Stops.TryRemove(existing.Id, out var removed);

					return GetRouteDetail(route.Symbol);
				}
				return null;
			}

		    public RouteViewModel[] GetNearbyRoutes(string stop)
		    {
			    var route = _region.Routes.Values.FirstOrDefault(x => x.Stops.ContainsKey(stop));
			    if (route != null)
			    {
				    var prox = route.Proximity;
				    return _region.Routes.Values.Where(
						    x => x.Status != RouteStatus.Complete && x.Type != RouteType.OnCall && x.Symbol != route.Symbol)
					    .OrderBy(x => Math.Abs(x.Proximity - prox))
					    .Select(s => (RouteViewModel)s)
					    .ToArray();
			    }

				return new RouteViewModel[0];
			}
	    }

	    public object Get()
	    {
		    var task = Request.RequestUri.Segments.LastOrDefault().ToUpper();

			switch (task)
		    {
				case "ROUTES":
					var result = _manager.GetRouteList().Select(s => (RouteViewModel) s.Value)
						.OrderBy(x => x.Symbol)
						.ToList();
					return result;

				case "REGION":
					return _manager.GetRegion();

				case "NEARBY":
					var query = Request.RequestUri.ParseQueryString();
					var stop = query["stop"];
					return _manager.GetNearbyRoutes(stop.ToUpper());

				default:
					var manifest = _manager.GetRouteDetail(task);
					if (manifest == null) return "PING";
					return manifest;
			}
	    }

	    [HttpPost]
	    public object Transfer( TransferRequest request )
	    {
		    return _manager.AssignOnCall(request.StopId, request.ToRouteId);
	    }
	}
}
