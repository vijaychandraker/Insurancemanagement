using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Dbord
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Prevent caching for all pages
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
           
            var context = HttpContext.Current;
            if (context == null) return;

            string path = context.Request.Path.ToLower();

            // Allow login page and static resources without session
            if (path.Contains("/login/login.aspx") ||
                path.Contains("/assets/") ||
                path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".png") || path.EndsWith(".jpg"))
            {
                return;
            }

            // Check session
            if (context.Session != null && context.Session["RoleID"] == null)
            {
                context.Response.Redirect("~/login/Login.aspx");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}