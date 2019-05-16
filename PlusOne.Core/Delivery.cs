namespace PlusOne.Core
{
	using System;

	public class Delivery : Stop
	{
		public DateTime? Delivered { get; set; }

		public Delivery() : base()
		{
			Type = StopType.Delivery;
		}
	}
}