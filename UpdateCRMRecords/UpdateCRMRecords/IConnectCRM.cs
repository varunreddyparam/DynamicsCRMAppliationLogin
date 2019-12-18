using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace DynamicsCRMConnection
{
    public interface IConnectCRM
    {
       Service ConnectionService(string URL, string userName, string password);
      
    }
}
