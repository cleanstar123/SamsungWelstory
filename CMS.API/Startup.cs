using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(CMS.API.Startup))]

namespace CMS.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 응용 프로그램을 구성하는 방법에 대한 자세한 내용은 https://go.microsoft.com/fwlink/?LinkID=316888을 참조하세요.

            ConfigureAuth(app);
        }

        private static void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/account/login"),

            });
        }
    }
}

