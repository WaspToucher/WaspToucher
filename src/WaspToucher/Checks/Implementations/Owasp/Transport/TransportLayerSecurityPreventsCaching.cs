using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaspToucher.Checks.Implementations.Owasp.Transport
{
    public class TransportLayerSecurityPreventsCaching : IPassiveCheck
    {
        public string Description
        {
            get
            {
                return "";
            }
        }

        public string Name
        {
            get
            {
                return "Prevent Caching of Sensitive Data";
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
                return new Uri("https://www.owasp.org/index.php/Transport_Layer_Protection_Cheat_Sheet#Rule_-_Prevent_Caching_of_Sensitive_Data");
            }
        }

        public PassiveCheckResult RunCheck(Fiddler.Session fiddlerSession)
        {
            if (fiddlerSession.isHTTPS)
            {
                if (fiddlerSession.oResponse.headers.Exists("cache-control"))
                {
                    string cc = fiddlerSession.oResponse.headers["cache-control"].Trim().ToLower();
                    if (!cc.Contains("no-store"))
                    {
                        return PassiveCheckResult.CreateFailure(this, fiddlerSession.url, "Cache-Control header does not contain 'no-store'");
                    }
                    else if (!cc.Contains("no-cache"))
                    {
                        return PassiveCheckResult.CreateFailure(this, fiddlerSession.url, "Cache-Control header does not contain 'no-cache'");
                    }
                }
                else
                {
                    return PassiveCheckResult.CreateFailure(this, fiddlerSession.url, "No Cache-Control header found");
                }
            }

            return PassiveCheckResult.CreatePass(this, fiddlerSession.url);
        }
    }
}