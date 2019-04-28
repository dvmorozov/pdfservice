using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Helpers;
using EnterpriseServices.Controllers;

namespace EnterpriseServices
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true; 

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/2ae0f902-4ecc-4792-ae8d-56f19d41bc91
                Session["ErrMessageStrings"] = ErrorHandlingController.GetErrorMessage(Server.GetLastError());
                Server.ClearError();
                Response.Redirect("/Pdf/Error");
            }
            catch
            { 
                //  Ignore possible exceptions.
            }
        }
    }
}
