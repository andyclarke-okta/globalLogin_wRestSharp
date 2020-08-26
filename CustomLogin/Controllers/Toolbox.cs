using Newtonsoft.Json.Linq;
//using Okta.Core;
//using Okta.Core.Clients;
//using Okta.Core.Models;
//using ClaytonCustomLogin.Models;
//using RestSharp;
//using RestSharp.Deserializers;
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
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace AICPACustomLogin.Controllers
{
    public class Toolbox
    {

        internal static string GotoDashboard(string sessionToken, string relayState, string location = null )
        {

            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            string destLink = null;
            // build redirectURL
            System.Text.StringBuilder redirectURL = new System.Text.StringBuilder();

            redirectURL.Append(appSettings["okta.OrgUrl"]);
            redirectURL.Append("/login/sessionCookieRedirect?token=" + sessionToken);


            if (!string.IsNullOrEmpty(location))
            {
                destLink = location;  // Assign  Org Base URL
            }
            else
            {
                destLink = appSettings["okta.OrgUrl"];  // Assign  Org Base URl
            }
            
 
            if (!string.IsNullOrEmpty(relayState))
            {
                var fixRelayState = new Regex("^\\/*").Replace(HttpUtility.UrlDecode(relayState), "/");             // Must fix relaystate with double "//" (forward slashes)
                destLink = destLink + fixRelayState;
            }
            string encodedCalURL = HttpUtility.UrlEncode(destLink);                                       // Encoding is nice

            redirectURL.Append("&redirectUrl=" + encodedCalURL);                                                // Supply link w/ relaystate for Hub application (CAL)                                          

            //Response.Redirect(redirectURL.ToString());
            return redirectURL.ToString();

        }


      

    }
}