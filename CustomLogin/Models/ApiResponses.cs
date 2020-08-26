using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AICPACustomLogin.Models
{


    public class UserValidateResponse
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string login { get; set; }
  
        public string status { get; set; }

        public bool mfaActive { get; set; }
    }

    



    /// <summary>
    /// ///////////////////////////////////////////////////////////
    /// </summary>
    public class userAuthnResponse
    {
        public string stateToken { get; set; }
        public string sessionToken { get; set; }
        public DateTime expiresAt { get; set; }
        public string status { get; set; }
        public _Embedded _embedded { get; set; }
        public _Links1 _links { get; set; }
    }

    public class _Embedded
    {
        public User user { get; set; }
        public Factor[] factors { get; set; }
        public Policy policy { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public Profile profile { get; set; }
    }

    public class Profile
    {
        public string login { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string locale { get; set; }
        public string timeZone { get; set; }
    }

    public class Policy
    {
        public bool allowRememberDevice { get; set; }
        public int rememberDeviceLifetimeInMinutes { get; set; }
        public bool rememberDeviceByDefault { get; set; }
    }

    public class Factor
    {
        public string id { get; set; }
        public string factorType { get; set; }
        public string provider { get; set; }
        public string vendorName { get; set; }
        public Profile1 profile { get; set; }
        public _Links _links { get; set; }
    }

    public class Profile1
    {
        public string phoneNumber { get; set; }
    }

    public class _Links
    {
        public Verify verify { get; set; }
    }

    public class Verify
    {
        public string href { get; set; }
        public Hints hints { get; set; }
    }

    public class Hints
    {
        public string[] allow { get; set; }
    }

    public class _Links1
    {
        public Cancel cancel { get; set; }
    }

    public class Cancel
    {
        public string href { get; set; }
        public Hints1 hints { get; set; }
    }

    public class Hints1
    {
        public string[] allow { get; set; }
    }

}