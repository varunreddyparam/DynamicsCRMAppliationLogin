using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Discovery;
using System.ServiceModel.Dispatcher;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System.Configuration;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Web;

namespace UpdateCRMRecords
{
    public class Service
    {
        private static Service _serviceInstance;
        private IOrganizationService _organizationService;
        private OrganizationWebProxyClient _organizationwebProxyClient;
        private Guid _controlID;
        private static object _lockObject = new object();
        public IOrganizationService OrganizationService
        {
            get { return _organizationService; }
        }
        public Guid ControlID
        {
            get { return _controlID; }
        }
        private Service(Guid controlID)
        {
            
            _controlID = controlID;

        }
        public static Service GetService()
        {
            try
            {
                if (_serviceInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_serviceInstance == null)
                            _serviceInstance = new Service(Guid.NewGuid());
                    }
                }
                return _serviceInstance;
            }
            catch (Exception ex)
            {
                XrmExceptionHandler exceptionHandlers = new XrmExceptionHandler();
                bool ErrorDetail = exceptionHandlers.HandleException(ex);
                exceptionHandlers.CreateLog(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
                    + " : " + System.Reflection.MethodBase.GetCurrentMethod().ToString()
                    + " : " + ErrorDetail, System.Diagnostics.EventLogEntryType.Error);
                return null;
            }
        }

        private string GetCRMToken()
        {
            var tenant = ConfigurationManager.AppSettings["tenantid"];
            var crmUrl = ConfigurationManager.AppSettings["crmurl"];
            var clientid = ConfigurationManager.AppSettings["appid"];
            var clientSecret = ConfigurationManager.AppSettings["secret"];



            var client = new RestClient($"https://login.microsoftonline.com/{tenant}/oauth2/token");
            var bodyString = $"grant_type=client_credentials&client_id={clientid}&client_secret={HttpUtility.UrlEncode(clientSecret)}&resource={HttpUtility.UrlEncode(crmUrl)}";
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", bodyString, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return JObject.Parse(response.Content)["access_token"].ToString();
        }

        public OrganizationWebProxyClient GetDynamicsSvc()
        {
            var svc = new OrganizationWebProxyClient(new Uri($"{ConfigurationManager.AppSettings["crmurl"]}/xrmservices/2011/organization.svc/web?SdkClientVersion=9.0"), false);
            svc.HeaderToken = GetCRMToken();
            svc.InnerChannel.OperationTimeout = new TimeSpan(9, 9, 9);
            return svc;
        }

        public void Connection()
        {
            ClientCredentials clntCredentials = new ClientCredentials();
            clntCredentials.UserName.UserName = "uambld@microsoft.com";
            clntCredentials.UserName.Password = "C)V_H.JNYa'#9br!";
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            //Security Protocol as TLS12
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //clntCredentials.Windows.ClientCredential = new System.Net.NetworkCredential("uambld", "C)V_H.JNYa'#9br!", "microsoft");
            //using (OrganizationServiceProxy proxy = new OrganizationServiceProxy(new Uri("https://uam.api.crm.dynamics.com/XRMServices/2011/Organization.svc"), null, clntCredentials, null))
            //{
            //    //proxy.ServiceConfiguration.CurrentServiceEndpoint.EndpointBehaviors.Add(new ProxyTypesBehavior());
            //    proxy.EnableProxyTypes();
            //    var xrmOrgService= (IOrganizationService)proxy;
            //};
            OrganizationServiceProxy orgService = new OrganizationServiceProxy(new Uri("https://uam.api.crm.dynamics.com/XRMServices/2011/Organization.svc"), null, clntCredentials, null);
            _organizationService = (IOrganizationService)orgService;
        }

    }
}
