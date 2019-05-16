#region Copyright © 2019, Warmoose Design Group
#endregion

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