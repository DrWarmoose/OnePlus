#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Generator
{
	using System.Linq;
	using Core;

	public static class RegionGenerator
    {
        public static Region Create(string regionName, string regionSymbol)
        {
            var routes = AddressGenerator.Postals.Select(postal => RouteGenerator.Create(postal, 20, 20)).ToList();

            var region = new Region
            {
                Name = regionName,
                Symbol =  regionSymbol,
                Hub = AddressGenerator.Create(),
            };

            var onCall = OnCalls;
            region.Routes.TryAdd(onCall.Symbol, onCall);

            foreach (var route in routes)
                region.Routes.TryAdd(route.Symbol, route);
			  
            return region;
        }

        private static RouteManifest OnCalls
        {
            get
            {
                var onCallRoute = new RouteManifest
                {
                    Name = "On Calls",
                    Status = RouteStatus.OnCall,
                    Type = RouteType.OnCall,
                    Symbol = "STX",
                };

                foreach (var postal in AddressGenerator.Postals)
                {
                    for (var i = 1; i <= 3; i++)
                    {
                        var stop = new Stop
                        {
                            Type = StopType.OnCall,
                            Address = AddressGenerator.Create(postal, i),
							Name = PackageGenerator.CreateContact().Name
                        };
                        onCallRoute.Stops.TryAdd(stop.Id, stop);
                    }
                }

                return onCallRoute;
            }
        }
    }
}