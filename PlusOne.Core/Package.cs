#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Core
{
	public class Package
	{
		public string Airbill { get; set; }
		public Address Origin { get; set; }
		public Address Destination { get; set; }
		public Contact Shipper { get; set; }
		public Contact Recipient { get; set; }
		public Dimensions Dimensions { get; set; }
		public ServiceLevel Service { get; set; }
		public SignatureRequired Signature { get; set; }
	}
}