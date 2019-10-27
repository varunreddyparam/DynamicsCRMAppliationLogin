using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace UpdateCRMRecords
{
    public class UpdateRecord
    {
        public static void Main()
        {
            Service xrmService = Service.GetService();
            Console.WriteLine(xrmService.ControlID);
            if (xrmService.OrganizationService != null)
            {
                Guid userid = ((WhoAmIResponse)xrmService.OrganizationService.Execute(new WhoAmIRequest())).UserId;
                if (userid != Guid.Empty)
                {
                    Console.WriteLine("Connection Established Successfully...");
                }
            }
            else
            {
                Console.WriteLine("Failed to Established Connection!!!");
            }
        }
    }
}
