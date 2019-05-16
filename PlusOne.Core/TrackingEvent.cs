namespace PlusOne.Core
{
	using System;

	public class TrackingEvent
	{
		public TrackingEventType Type { get; set; }
		public DateTime TimeStamp { get; set; }
		public Employee Actor { get; set; }
	}
}