﻿using System;
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

        private Service(Guid controlID, string URL, string userName,string password)
        {

            _controlID = controlID;
            _organizationService = this.Connectwithcreds(URL, userName, password);
            _organizationwebProxyClient = this.GetDynamicsSvc();
        }
        public static Service GetService(string URL, string userName, string password)
        {
            try
            {
                if (_serviceInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_serviceInstance == null)
                            _serviceInstance = new Service(Guid.NewGuid(), URL, userName, password);
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

        public IOrganizationService Connectwithcreds(string URL, string userName, string password)
        {
            ClientCredentials clntCredentials = new ClientCredentials();
            clntCredentials.UserName.UserName = userName;
            clntCredentials.UserName.Password = password;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            //Security Protocol as TLS12
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string url = string.Format("{0}/XRMServices/2011/Organization.svc", URL);
            OrganizationServiceProxy orgService = new OrganizationServiceProxy(new Uri(url), null, clntCredentials, null);
            return (IOrganizationService)orgService;

        }

    }
}
