/*
 “Commons Clause” License Condition v1.0
The Software is provided to you by the Licensor under the License, as defined below, subject to the following condition.
Without limiting other conditions in the License, the grant of rights under the License will not include, and the License
does not grant to you, right to Sell the Software. For purposes of the foregoing, “Sell” means practicing any or all of
the rights granted to you under the License to provide to third parties, for a fee or other consideration (including
without limitation fees for hosting or consulting/ support services related to the Software), a product or service whose
value derives, entirely or substantially, from the functionality of the Software.  Any license notice or attribution
required by the License must also include this Commons Cause License Condition notice.

Software: PdfService

License: 
Copyright 2020 Dmitry Morozov

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Licensor: Dmitry Morozov
 */
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
            List<string> mess = new List<string>();
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
                string mess =   "Error in Path :" + Request.Path;                         //Get the path of the page
                mess += "\n\n Error Raw Url :" + Request.RawUrl;                          //Get the QueryString along with the Virtual Path
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
