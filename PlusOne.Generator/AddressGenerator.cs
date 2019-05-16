#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Generator
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Core;

	/// <summary>
    /// the addresses really need to be San Antonio addresses otherwise the mapping software
    /// will freak out due to inability to geolocate the bogus street address
    /// </summary>
    public class AddressGenerator
    {
        private static string PoolAddressPath = "STXPool.txt";
        private static Random _random = new Random();
        
        private static readonly Dictionary<string,List<string>> _pool;
        //private static Dictionary<string, List<string>> Pool = _pool ?? (_pool = LoadPool());
        private static Dictionary<string, List<string>> Pool = _pool ?? (_pool = UserData.Street);

		public static string PoolAddress 
        {
            get
            {
                var here = Assembly.GetExecutingAssembly().CodeBase;
                var directory = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(here).Path));
                return Path.Combine(directory, PoolAddressPath);
            }
        }

        public static string[] Postals => Pool.Keys.ToArray();

        public static Dictionary<string, List<string>> LoadPool()
        {
            var pool = new Dictionary<string, List<string>>();
            var path = PoolAddress;

            if (!File.Exists(path))
            {
	            var list = new List<string>();
	            for (var x = 0; x < 10; x++)
	            {
		            try
		            {
			            foreach (var file in Directory.EnumerateFiles("..\\bin"))
				            list.Add(file);
		            }
		            catch (Exception e)
		            {
						list.Add(e.Message);
						list.Add(e.Message);
						list.Add(e.Message);
		            }
				}

	            pool.Add("00000", list);
	            return pool;
            }

            try
            {
	            foreach (var line in File.ReadAllLines(path))
	            {
		            var parts = line.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
		            if (parts.Length <= 1) continue;
		            if (!pool.TryGetValue(parts[0], out var list))
		            {
			            pool[parts[0]] = new List<string> {parts[1]};
		            }
		            else
		            {
			            pool[parts[0]].Add(parts[1]);
		            }
	            }

	            foreach (var pp in pool)
	            {
		            Debug.Write($"\t{{\"{pp.Key}\", new List<string>{{ ");
		            foreach (var pv in pp.Value)
		            {
						Debug.Write( $"\"{pv}\", ");
		            }
					Debug.WriteLine( $"\t}} }},");
	            }
            }
            catch (Exception ex)
            {
	            throw ex;
            }

            return pool;
        }

        public static Address Create()
        {
            var zips = Pool.Keys.ToArray();
            return Create(zips[_random.Next(zips.Length)]);
        }
        
        public static Address Create(string postal, int onCalls = 0 )
        {
            string street = "Address on Package";
            
            if (Pool.TryGetValue(postal, out var list))
            {
                var index = onCalls==0 ? _random.Next(0, list.Count-3) 
                    : list.Count - onCalls;
                
                street = list[index];
            }
            
            return new Address
            {
                Street = new []{street},
                City = "San Antonio",
                Postal = postal,
                State = "TX"
            }; 
        }
    }
}