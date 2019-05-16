namespace PlusOne.Generator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;

	public class RouteGenerator
    {
	    static int cycle = 0;

		public static RouteManifest Create(string postal, int pickups, int deliveries )
		{
            var manifest = new RouteManifest
            {
                Symbol = $"STX-{postal.Substring(postal.Length-2)}", 
                Name = $"San Antonio - {postal}",
				Driver = GetDriver(),
				Status = RouteStatus.Enroute	,
				Proximity = int.TryParse(postal, out var code) ? code : 0
			};

			if (pickups > 0)
				manifest.Type |= RouteType.Pickup;

			if (deliveries > 0)
				manifest.Type |= RouteType.Delivery;

			var list = new List<Stop>();
            list.AddRange( GetDeliveries(postal,deliveries));
            list.AddRange( GetPickups(postal,pickups));
            var ordered = list.OrderBy(x => x.Type)
                .ThenBy(x => x.ServiceLevel);

            var order = 1;
            foreach (var stop in ordered)
            {
                stop.Order = order++;
                manifest.Stops.TryAdd(stop.Id, stop);
            }

            return manifest;
        }

        public static string GetDriver()
        {
	        string[] drivers =
	        {
		        "Reuben Hick", "Dusty Rhodes", "Justin Tyme", "Helen Bach", "Rusty Pipes", "Lauren Order",
				"Bjorn Free", "Doug Graves", "Dylan Weed", "Eve Hill", "Bess Twishes", "Ira Fuse", "Gene Poole",
				"Ben Dover", "Phil MacCrevice", "Homan Provement", "Hugo First", "Moe Telsiks", "Wayne Deer",
				"Mary Thon", "Kim Payne", "Juan Fortharoad", "Hugh deMann", "Brook Lynn Bridge"
	        };

	        var driver = drivers[cycle];
			cycle = (cycle + 1) % drivers.Length;
			return driver;
        }

        public static Pickup[] GetPickups(string postal, int count )
        {
            var unique = new Dictionary<string, Pickup>();
            
            while (count-- >= 0)
            {
                var package = PackageGenerator.CreatePickup(postal);
                if ( ! unique.TryGetValue(package.Origin.ToString(), out var existing))
                {
                    existing = new Pickup();
                    unique.Add(package.Origin.ToString(),existing);
                }

                existing.Packages.TryAdd(package.Airbill,package);
            }

            return unique.Values.ToArray();
        }

        public static Delivery[] GetDeliveries(string postal, int count )
        {
            var unique = new Dictionary<string, Delivery>();
            
            while (count-- >= 0)
            {
                var package = PackageGenerator.CreateDelivery(postal);
                if ( ! unique.TryGetValue(package.Destination.ToString(), out var existing))
                {
                    existing = new Delivery();
                    unique.Add(package.Destination.ToString(),existing);
                }

                existing.Packages.TryAdd(package.Airbill,package);
            }

            return unique.Values.ToArray();
        }
    }
}