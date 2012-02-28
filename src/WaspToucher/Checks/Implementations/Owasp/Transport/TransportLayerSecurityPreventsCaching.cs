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
                //if (fiddlerSession.)
                //{
                //    return PassiveCheckResult.CreateFailure(this, fiddlerSession.url);
                //}
                throw new NotImplementedException();
            }

            return PassiveCheckResult.CreatePass(this, fiddlerSession.url);
        }
    }
}