using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;

namespace WaspToucher.Checks.Implementations.Owasp.Transport
{
    public class TransportLayerSecurityForLogin : IPassiveCheck
    {
        public string Description
        {
            get
            {
                return "Use TLS for All Login Pages";
            }
        }

        public string Name
        {
            get
            {
                return "TLS used with Login pages";
            }
        }

        public ComplianceStandard[] Compliances
        {
            get
            {
                return new ComplianceStandard[] { ComplianceStandard.Owasp };
            }
        }

        public Uri InformationUrl
        {
            get
            {
                return new Uri("https://www.owasp.org/index.php/Transport_Layer_Protection_Cheat_Sheet#Rule_-_Use_TLS_for_All_Login_Pages_and_All_Authenticated_Pages");
            }
        }

        public PassiveCheckResult RunCheck(Session fiddlerSession)
        {
            if (!fiddlerSession.isHTTPS)
            {
                if (fiddlerSession.uriContains("login"))
                {
                    return PassiveCheckResult.CreateFailure(this, fiddlerSession.url);
                }
            }

            return PassiveCheckResult.CreatePass(this, fiddlerSession.url);
        }
    }
}