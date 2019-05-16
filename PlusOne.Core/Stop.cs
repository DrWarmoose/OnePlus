namespace PlusOne.Core
{
	using System.Collections.Concurrent;
	using System.Linq;

	public class Stop
	{
		private static int _id;
		private Address _address;
		private string _name;

		public ConcurrentDictionary<string, Package> Packages { get; } = new ConcurrentDictionary<string, Package>();
		public ConcurrentQueue<Visit> Attempts { get; } = new ConcurrentQueue<Visit>();
		public StopType Type { get; set; }
		public string Id { get; }
		public int Order { get; set; }

		public string Name
		{
			get => _name ?? (_name = GetName());
			set => _name = value;
		}

		public Address Address
		{
			get
			{
				if (_address != null)
				{
					return _address;
				}

				return Type == StopType.Delivery ? Packages.Values.FirstOrDefault()?.Destination :
					Packages.Values.FirstOrDefault()?.Origin;
			}
			set => _address = value;
		}

		public ServiceLevel ServiceLevel
		{
			get { return Packages.Any() ? Packages.Values.Min(x => x.Service) : ServiceLevel.SecondDay; }
		}

		public Stop()
		{
			Id = (_id++).ToString().PadLeft(8, '0');
		}

		string GetName()
		{
			Package package;
			if (Type == StopType.Delivery)
			{
				package = Packages.Values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Recipient?.Name));
				if (package != null)
				{
					return package.Recipient.Name;
				}
			}
			package = Packages.Values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Shipper?.Name));
			if (package != null)
			{
				return package.Recipient.Name;
			}

			return "Name On Package";
		}

		public override string ToString()
		{
			var count = Packages.Count;
			return $"{ServiceLevel}: {count} packages";
		}
	}
}