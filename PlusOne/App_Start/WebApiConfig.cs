namespace PlusOne
{
	using System.Web.Http;
	using System.Web.Http.Cors;

	public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
			// Web API configuration and services
			var policy = new EnableCorsAttribute("*", "*", "*");
			config.EnableCors(policy);

			// Web API routes
			config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
