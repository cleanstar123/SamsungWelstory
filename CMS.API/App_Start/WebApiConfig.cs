using System.Web.Http;

namespace CMS.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 라우팅 활성화
            config.MapHttpAttributeRoutes();

            // 기본 라우트 설정
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
