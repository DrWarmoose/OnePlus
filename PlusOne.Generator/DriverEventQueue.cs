namespace PlusOne.Generator
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using Core;

	public class DriverEventQueue
    {
        private readonly ConcurrentQueue<DriverEvent> _queue = new ConcurrentQueue<DriverEvent>();
        private readonly string _routeSymbol;

        public DriverEventQueue(string routeSymbol)
        {
            _routeSymbol = routeSymbol.Trim();
        }

        public DateTime Mark { get; set; }

        public DriverEventQueue Add(TrackingEventType action, int minutes = 0)
        {
            Mark = Mark.AddMinutes(minutes); 
            _queue.Enqueue( new DriverEvent
            {
                Action = action,
                When = Mark,
                RouteSymbol = _routeSymbol
            });
            return this;
        } 
        
        public DriverEventQueue Add(TrackingEventType action, Visit visit, int minutes = 0)
        {
            Mark = Mark.AddMinutes(minutes); 
            _queue.Enqueue( new DriverEvent
            {
                Action = action,
                When = Mark,
                Visit = visit,
                RouteSymbol = _routeSymbol
            });
            return this;
        }

        public DriverEvent AddDriverEvent(TrackingEventType action, Visit visit, int minutes = 0)
        {
            Mark = Mark.AddMinutes(minutes);
            var ev = new DriverEvent
            {
                Action = action,
                When = Mark,
                Visit = visit,
                RouteSymbol = _routeSymbol
            };
            _queue.Enqueue(ev);
            
            return ev;
        }

        public IEnumerable<DriverEvent> AllEvents()
        {
            while (!_queue.IsEmpty && _queue.TryDequeue(out var de))
                yield return de;
        }
        
        public IEnumerable<DriverEvent> RemainingEvents()
        {
            var now = DateTime.UtcNow;
            while (!_queue.IsEmpty && _queue.TryDequeue(out var de))
            {
                if ( de.When > now )
                    yield return de;
            }
        }
    }
}