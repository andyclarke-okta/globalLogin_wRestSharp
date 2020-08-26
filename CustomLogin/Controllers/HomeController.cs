using Newtonsoft.Json.Linq;
//using Okta.Core;
//using Okta.Core.Clients;
//using Okta.Core.Models;
using RestSharp;
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
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using AICPACustomLogin.Models;



namespace AICPACustomLogin.Controllers
{
    public class HomeController : Controller
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // ILog logger = LogManager.GetLogger("SpecialLogFile");

        NameValueCollection appSettings = ConfigurationManager.AppSettings;

            

        public ActionResult Help()
        {
            return View("LoginHelp");
        }

        public ActionResult AltLanding()
        {
            TempData["oktaOrg"] = appSettings["okta.OrgUrl"];
            TempData["token"] = appSettings["okta.OrgToken"];
            return View("AltLanding");
        }


        [HttpGet]
        public ActionResult Index(string passName)
        {
            logger.Debug("Home/Index GET ");
            string myStatus;
            string myStateToken;
            string mySessionToken;
            string myRelayState;
            string myOktaId;

            // handle relaystate and messages
            myRelayState = null;

            if ((string)TempData["relayState"] != null)
            {
                myRelayState = (string)TempData["relayState"];
            }
            else if (Request.QueryString["RelayState"] != null)
            {
                myRelayState = Request.QueryString["RelayState"];

                if (myRelayState != null)
                {
                    if (!myRelayState.Contains("%2F%2Flogin%2Flogin.htm"))
                    {
                        myRelayState = null;
                    }
                }
            }
            else
            {
                myRelayState = Request["RelayState"];

                if (myRelayState != null)
                {
                    if (myRelayState == "%2F%2Flogin%2Flogin.htm")
                    {
                        myRelayState = null;
                    }
                }

            }

            ViewData["relayState"] = myRelayState;
            //TempData["errMessage"] = "place error or feedback messages here";
            TempData["userName"] = passName;
            ViewData["HelpLink"] = appSettings["aicpa.helpUrl"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ProcessSession()
        {
            logger.Debug("Home/Logout POST ");
            string session_data = Request["session_data"];
            string logout_but = Request["logout_but"];
            string validate_but = Request["validate_but"];

            string myStatus = null;
            string myStateToken = null;
            string mySessionToken = null;
            string mySessionId = null;


            if (validate_but == "Validate User Session")
            {
                if (string.IsNullOrEmpty(session_data))
                {
                    TempData["errMessage"] = "Session ID is NUll";
                }
                else
                {
                    var client = new RestClient(appSettings["okta.OrgUrl"] + "/api/v1/sessions/" + session_data);
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Accept", "application/json");
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", " SSWS " + appSettings["okta.OrgToken"]);
                    IRestResponse<UserValidateResponse> response = client.Execute<UserValidateResponse>(request);
                    myStatus = response.Data.status;
                    mySessionId = response.Data.id;

                    if (!string.IsNullOrEmpty(mySessionId))
                    {
                        TempData["errMessage"] = "Validate Session Good; sid " + mySessionId;
                    }
                    else
                    {
                        TempData["errMessage"] = "Validate Session Bad";
                    }
                }

            }

            TempData["oktaOrg"] = appSettings["okta.OrgUrl"];
            TempData["token"] = appSettings["okta.OrgToken"];
            return View("AltLanding");
        }




        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Route()
        {
            string myStatus = null;
            string myStateToken= null;
            string mySessionToken = null;
            string myRelayState;
            string myOktaId = null;
            bool secQuestionSet = true;
            string rspUserStatus = null;
  

            // get form post parameters
            string userName = Request["txtUserName"];
            string passWord = Request["txtPassword"];
            string relayState = Request["relayState"];
            string rememberMe = Request["optRemember"];
            string location = Request["location"];
     
            logger.Debug("Home-Route  POST User: " + userName + " RelayState: " + relayState);

            // set relayState to query param if post param is null or blank
            if (!string.IsNullOrEmpty(relayState))
            {
                myRelayState = Request.QueryString["RelayState"];
                TempData["relayState"] = myRelayState;
            }
            else
            {
                myRelayState = relayState;
                TempData["relayState"] = relayState;
            }

            // redirect if missing username
            if (string.IsNullOrEmpty(userName))
            {
                // made it this far must not have account
                TempData["errMessage"] = "Username Missing";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(passWord))
            {

                TempData["errMessage"] = "Password Missing";
                return RedirectToAction("Index");
            }

            
            // implement authn process
            try
            {
               
                UserAuthnRequest  userAuthnRequest = new UserAuthnRequest();
                userAuthnRequest.username = userName;
                userAuthnRequest.password = passWord;

            
                var client = new RestClient(appSettings["okta.OrgUrl"] + "/api/v1/authn");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("Authorization", " SSWS " + appSettings["okta:OrgToken"]);
                request.AddJsonBody(userAuthnRequest);
                //request.AddParameter("application/json", "{\n    \"username\": \" " + userName + "\" ,\"password\":\"" + passWord + "\"}", ParameterType.RequestBody);
                IRestResponse<userAuthnResponse>  response = client.Execute<userAuthnResponse>(request);

                myStatus = response.Data.status;
                mySessionToken = response.Data.sessionToken;
                myStateToken= response.Data.stateToken;


            }
            catch (Exception ex)
            {

                    logger.Error("Sign in process failed!");
                    // generic failure
                    TempData["errMessage"] = "Sign in process failed!";
                
                TempData["userName"] = userName;
                //return RedirectToAction("Index");
                return View("Index");
            }//end catch

            switch (myStatus)
            {

                case "PASSWORD_WARN":  //password about to expire
                    logger.Debug("PASSWORD_WARN ");
                    //no action required
                    break;

                case "PASSWORD_EXPIRED":  //password has expired
                    logger.Debug("PASSWORD_EXPIRED ");
                    break;

                case "RECOVERY":  //user has requested a recovery token
                    logger.Debug("RECOVERY ");
                    //find which recovery mode sms, email is being used
                    //POST to next link
                    break;

                case "RECOVERY_CHALLENGE":  //user must verify factor specific recovery challenge
                    logger.Debug("RECOVERY_CHALLENGE ");
                    //verify the recovery factor
                    //POST to verify link
                    break;

                case "PASSWORD_RESET":     //user satified recovery and must now set password
                    logger.Debug("PASSWORD_RESET ");

                    //reset users password
                    //POST to next link
                    break;

                case "LOCKED_OUT":  //user account is locked, unlock required
                    logger.Debug("LOCKED_OUT ");
                    break;

                case "MFA_ENROLL":   //user must select and enroll an available factor 
                    logger.Debug("MFA_ENROLL ");
                    break;

                case "MFA_ENROLL_ACTIVATE":   //user must activate the factor to complete enrollment
                    logger.Debug("MFA_ENROLL_ACTIVATE ");
                    //user must activate the factor
                    //POST to next link
                    break;

                case "MFA_REQUIRED":    //user must provide second factor with previously enrolled factor
                    logger.Debug("MFA_REQUIRED ");
                    break;

                case "MFA_CHALLENGE":      //use must verify factor specifc challenge
                    logger.Debug("MFA_CHALLENGE ");
                    break;

                case "SUCCESS":      //authentication is complete
                    logger.Debug("SUCCESS");
                    string landingPage = null;
                    landingPage = location + "/Home/AltLanding";
                    //landingPage = location + "/Home/Sessions";
                    string redirectUrl = Toolbox.GotoDashboard(mySessionToken, myRelayState, landingPage);



                    return Redirect(redirectUrl);
                //break;
                default:
                    logger.Debug("Status: " + myStatus);
                    TempData["errMessage"] = "Status: " + myStatus;
                    break;
            }//end of switch

            return RedirectToAction("Index");
        }


    }
}