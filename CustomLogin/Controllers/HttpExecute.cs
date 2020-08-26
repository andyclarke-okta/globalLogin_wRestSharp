using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;


namespace AICPACustomLogin.Controllers
{

    

    public class SessionRequest
    {
        [JsonProperty("sessionToken")]
        public string sessionToken { get; set; }
    }

    public class CustomHttp
    {


        public static async Task RunGetAsync(string uri)
        {
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var headers = response.Headers.ToString();
                    
                }
            }
        }


        public static async Task RunPostAsync(string relativeUri, string sessionToken)
        {
            ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            using (var client = new HttpClient())
            {
   
                client.BaseAddress = new Uri(appSettings["okta.OrgUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "SSWS " + appSettings["okta.OrgToken"]);

                // HTTP POST
                var myRequest = new SessionRequest() { sessionToken = sessionToken };
                HttpResponseMessage response = await client.PostAsJsonAsync(relativeUri, myRequest);
                if (response.IsSuccessStatusCode)
                {
                    // Get the URI of the created resource.
                    Uri requestUrl = response.Headers.Location;
                }
            }
        }

        public static async Task RunDeleteAsync(string relativeUri, string sessionToken)
        {
            ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            NameValueCollection appSettings = ConfigurationManager.AppSettings;


            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(appSettings["okta.OrgUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "SSWS " + appSettings["okta.OrgToken"]);

                // HTTP Delete
                var myRequest = new SessionRequest() { sessionToken = sessionToken };
                HttpResponseMessage response = await client.DeleteAsync(relativeUri);
                if (response.IsSuccessStatusCode)
                {
                    // Get the URI of the created resource.
                    Uri requestUrl = response.Headers.Location;
                }
            }
        }



    }
}