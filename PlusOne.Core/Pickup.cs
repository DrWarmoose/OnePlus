namespace PlusOne.Core
{
	using System;

	public class Pickup : Stop
	{
		public bool Visible { get; set; }
		public DateTime? PickedUp { get; set; }

		public Pickup() : base()
		{
			Type = StopType.Pickup;
		}
	}
}