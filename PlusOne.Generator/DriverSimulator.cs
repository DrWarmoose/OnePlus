namespace PlusOne.Generator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;

	public class DriverSimulator
    {
        private static readonly Random _random = new Random();
        private readonly RouteManifest _manifest;
        private readonly DriverEventQueue _queue;
        
        public DriverSimulator(RouteManifest manifest)
        {
            _queue = new DriverEventQueue(manifest.Symbol);
            _manifest = manifest;
            _manifest.Status = RouteStatus.AtHub;
            _manifest.Current = null;
        }

        public IEnumerable<DriverEvent> AllEvents()
        {
            return _queue.AllEvents();
        }

        public IEnumerable<DriverEvent> RemainingEvents()
        {
            return _queue.RemainingEvents();
        }
        
        public void AssignRoute(DriverEvent e)
        {
            _manifest.Status = RouteStatus.AtHub;
            _manifest.Current = null;
        }

        public void InTransit(DriverEvent e)
        {
            _manifest.Status = RouteStatus.Enroute;
            _manifest.Current = null;
        }

        public void ReturnToHub(DriverEvent e)
        {
            _manifest.Status = RouteStatus.ReturnToHub;
            _manifest.Current = null;
        }

        public void ArriveHub(DriverEvent e)
        {
            _manifest.Status = RouteStatus.AtHub;
            _manifest.Current = null;
        }

        /// <summary>
        /// Idempotent stop assignment.
        /// </summary>
        /// <param name="visit"></param>
        public void ArriveStop( DriverEvent e )
        {
            _manifest.Status = RouteStatus.ArriveStop;
            _manifest.Current = e.Visit;
        }

        public void DepartStop(DriverEvent e)
        {
            _manifest.Status = RouteStatus.DepartStop;
            _manifest.Current = null;
            var stop = e.Visit?.Stop;
            if (stop != null)
            {
                if (stop is Pickup pickup)
                {
                    if (!pickup.PickedUp.HasValue)
                    {
                        pickup.PickedUp = e.When;
                    }
                }
                else if ( stop is Delivery delivery )
                {
                    if (!delivery.Delivered.HasValue)
                    {
                        delivery.Delivered = e.When;
                    }
                }
            }
        }

        public void Close(DriverEvent e)
        {
            _manifest.Status = RouteStatus.Complete;
            _manifest.Current = null;
        }

        public IEnumerable<DriverEvent> AssignOnCall( Stop stop )
        {
            stop.Order = _manifest.Stops.Values.Max(x => x.Order) + 100;
            _manifest.Stops.TryAdd(stop.Id, stop);
            return EnqueueStop(stop);
        }

        public void SimulateDay(DateTime start)
        {
            _queue.Mark = start;

            if (_manifest.Type.HasFlag(RouteType.OnCall))
            {
                // if on-call, just register the on-call 
                foreach (var stop in _manifest.Stops.Values.OrderBy(x => x.Order))
                {
                    _queue.Add(TrackingEventType.RequestForPickup, _random.Next(5,8));
                }

                return;
            }
            
            _queue.Add( TrackingEventType.Binning )
                .Add(TrackingEventType.OutForDelivery,10);

            foreach (var stop in _manifest.Stops.Values.OrderBy(x => x.Order))
            {
                  EnqueueStop(stop).ToArray();
            }

            _queue.Add( TrackingEventType.InTransit )
                .Add(TrackingEventType.ArriveHub, 15)
                .Add(TrackingEventType.Close,10);
        }

        private IEnumerable<DriverEvent> EnqueueStop(Stop stop)
        {
            var visit = new Visit{Stop = stop};
            yield return _queue.AddDriverEvent(TrackingEventType.AtDestination, visit, _random.Next(6) + 2);
            yield return _queue.AddDriverEvent(TrackingEventType.LeaveDestination, visit, _random.Next(6) + 2);

            // TODO: randomly select a visit that does not result in a pickup or delivery to reschedule.
        }
    }

    public class DriverEvent
    {
        public string RouteSymbol { get; set; } 
        
        public TrackingEventType Action { get; set; }
        public Visit Visit { get; set; }
        public DateTime When { get; set; }

        public override string ToString()
        {
            var street = Visit?.Stop?.Address?.Street?.FirstOrDefault();
            return $"{RouteSymbol} | {When}: {Action}  {street}";
        }
    }
}