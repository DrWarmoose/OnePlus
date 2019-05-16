#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Controllers
{
	using System.Linq;
	using System.Net.Http;
	using System.Web.Http;
	using Models;

	public class RouteController : ApiController
	{
		private IRouteManager _manager;

		public RouteController(IRouteManager routeManager = null)
		{
			_manager = routeManager ?? new RouteManager();
		}

	    public object Get()
	    {
		    var task = Request.RequestUri.Segments.LastOrDefault().ToUpper();

			switch (task)
		    {
				case "ROUTES":
					var result = _manager.GetRouteList().Select(s => (RouteViewModel) s.Value)
						.OrderBy(x => x.Symbol)
						.ToList();
					return result;

				case "REGION":
					return _manager.GetRegion();

				case "NEARBY":
					var query = Request.RequestUri.ParseQueryString();
					var stop = query["stop"];
					return _manager.GetNearbyRoutes(stop.ToUpper());

				default:
					var manifest = _manager.GetRouteDetail(task);
					if (manifest == null) return "PING";
					return manifest;
			}
	    }

	    [HttpPost]
	    public object Transfer( TransferRequest request )
	    {
		    return _manager.AssignOnCall(request.StopId, request.ToRouteId);
	    }
	}
}
