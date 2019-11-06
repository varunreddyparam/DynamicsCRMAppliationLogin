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
    public class ConnecttoCRM
    {
        public Service connection(string URL, string userName, string password)
        {
            Service xrmService = Service.GetService(URL,userName,password);
            return xrmService;
        }
    }
}
