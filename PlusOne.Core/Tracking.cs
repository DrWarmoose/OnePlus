namespace PlusOne.Core
{
	using System.Collections.Concurrent;

	public class Tracking
	{
		public ConcurrentQueue<TrackingEvent> Events { get; } = new ConcurrentQueue<TrackingEvent>();
	}
}