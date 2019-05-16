#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne
{
	using Core;
	using Generator;
	using Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public interface IRouteManager
	{
		Region GetRegion();
		RouteDetailModel[] GetRouteDetail(string symbol);
		Dictionary<string, RouteManifest> GetRouteList();
		RouteDetailModel[] AssignOnCall(string stop, string toRoute);
		RouteViewModel[] GetNearbyRoutes(string stop);
	}

	public class RouteManager : IRouteManager
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
					.Select(map => (RouteDetailModel)map).ToArray();
			}
			return null;
		}

		public Dictionary<string, RouteManifest> GetRouteList()
		{
			var mapping = new Dictionary<string, RouteManifest>();
			foreach (var map in _region.Routes)
				mapping.Add(map.Key, map.Value);

			return mapping;
		}

		public RouteDetailModel[] AssignOnCall(string stop, string toRoute)
		{
			//_sim.AssignOnCall(oncall, route);
			//return _region;

			var route = _region.Routes.Values.FirstOrDefault(x => x.Stops.ContainsKey(stop));
			if (route != null &&
				_region.Routes.TryGetValue(toRoute, out var transfer) &&
				route.Stops.TryGetValue(stop, out var existing))
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
}