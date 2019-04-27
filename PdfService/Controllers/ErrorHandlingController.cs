using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EnterpriseServices.Controllers
{
    public class ErrorHandlingController : Controller
    {
        //  https://www.evernote.com/shard/s132/nl/14501366/2ae0f902-4ecc-4792-ae8d-56f19d41bc91
        public static List<string> GetErrorMessage(Exception e)
        {
            var mess = new List<string>();
            if (e != null)
            {
                //mess.Add("Error Message :" + e.Message);                                //Get the error message
                //mess.Add("Error Source :" + e.Source);                                  //Source of the message
                //mess.Add("Error Stack Trace :" + e.StackTrace);                         //Stack Trace of the error
                //mess.Add("Error TargetSite :" + e.TargetSite);                          //Method where the error occurred
                mess.Add("Exception Message :" + (e.InnerException != null ? e.InnerException.Message : e.Message));
            }
            return mess;
        }

        protected void Application_Error(Exception e)
        {
            try
            {
                var mess =   "Error in Path :" + Request.Path;                          //Get the path of the page
                mess += "\n\n Error Raw Url :" + Request.RawUrl;                        //Get the QueryString along with the Virtual Path
                mess += "\n\n " + string.Join("\n\n ", GetErrorMessage(e).ToArray());
                //var myLog = new System.Diagnostics.EventLog { Source = "Expenses" };    //Create a new EventLog object
                //myLog.WriteEntry(mess, System.Diagnostics.EventLogEntryType.Error);     //Write the log
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception /*e1*/)
            // ReSharper restore EmptyGeneralCatchClause
            {
                //  Blocks all possible internal exceptions.
                //  https://www.evernote.com/shard/s132/nl/14501366/f71fe1ce-cc76-4179-bc10-6834393cb3f2
                //var dummy = e1.Message;
            }
        }
    }
}
