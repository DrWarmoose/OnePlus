namespace PlusOne.Generator
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Core;

	public class FleetSimulator
    {
        private  Region _region;
        readonly ConcurrentQueue<DriverEvent> _events = new ConcurrentQueue<DriverEvent>();
        readonly Dictionary<string,DriverSimulator> _drivers = new Dictionary<string,DriverSimulator>();
        readonly ManualResetEvent _kill = new ManualResetEvent(false);
        readonly ManualResetEvent _killed = new ManualResetEvent(false);

        public FleetSimulator(Region region)
        {
            Initialize(region);
        }
        
        public void AssignOnCall(string oncall, string route)
        {
            var oncallRoute = _region.Routes.Values.FirstOrDefault(x => x.Type.HasFlag(RouteType.OnCall));
            
            if (_drivers.TryGetValue(route, out var existing) && oncallRoute != null && 
                oncallRoute.Stops.TryGetValue(oncall, out var stop) )
            {
                foreach (var ev in existing.AssignOnCall(stop))
                    _events.Enqueue(ev);
            }
        }
        
        public void Initialize( Region region )
        {
            _region = region;
            _drivers.Clear();
            var launch = DateTime.UtcNow.AddMinutes(-60);
            foreach (var route in region.Routes.Values)
            {
                var sim = new DriverSimulator(route);
                sim.SimulateDay( launch );
                launch = launch.AddMinutes(5);
                _drivers.Add( route.Symbol, sim);
            }
            Refresh();
        }

        public void Refresh()
        {
            _killed.Reset();
            _kill.Set();
            
            var tempEvents = new List<DriverEvent>();
            foreach (var driver in _drivers.Values)
            {
                tempEvents.AddRange(driver.AllEvents());
            }

            while (!_events.IsEmpty && _events.TryDequeue(out var e)) ;
            foreach( var e in tempEvents.OrderBy(x => x.When))
                _events.Enqueue(e);
            
            _killed.WaitOne(1000);
            _kill.Reset();

            Task.Run(Execute);
        }
        
        public void Execute()
        {
            var now = DateTime.UtcNow;

            foreach (var before in _events.Where(x => x.When < now))
            {
                ExecuteDriverEvent(before);
            }

            foreach (var after in _events.Where(x => x.When >= now))
            {
                var wait = after.When - DateTime.UtcNow;
                if ( ! _kill.WaitOne(wait))
                {
                    _killed.Set();
                    return;
                }
                ExecuteDriverEvent(after);
            }
        }

        private void ExecuteDriverEvent(DriverEvent e)
        {
            if (_drivers.TryGetValue(e.RouteSymbol, out var driver))
            {
                switch (e.Action)
                {
                    case TrackingEventType.Binning:
                        driver.AssignRoute(e);
                        break;
                    case TrackingEventType.OutForDelivery:
                        driver.InTransit(e);
                        break;
                    case TrackingEventType.AtDestination:
                        driver.ArriveStop(e);
                        break;
                    case TrackingEventType.LeaveDestination:
                        driver.DepartStop(e);
                        break;
                    case TrackingEventType.InTransit:
                        driver.ReturnToHub(e);
                        break;
                    case TrackingEventType.ArriveHub:
                        driver.ArriveHub(e);
                        break;
                    case TrackingEventType.Close:
                        driver.Close(e);
                        break;
                }
            }
        }
    }
}