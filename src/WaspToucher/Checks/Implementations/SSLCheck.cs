using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaspToucher.Checks.Implementations
{
    public class SSLCheck : IPassiveCheck
    {
        public string Description
        {
            get { return "Checks that SSL is being used"; }
        }

        public string Name
        {
            get { return "SSL used"; }
        }

        public ComplianceStandard[] Compliances
        {
            get { return new ComplianceStandard[] { ComplianceStandard.None }; }
        }

        public Uri InformationUrl
        {
            get { throw new NotImplementedException(); }
        }

        public PassiveCheckResult RunCheck(Fiddler.Session fiddlerSession)
        {
            return fiddlerSession.isHTTPS ? PassiveCheckResult.CreatePass(this, fiddlerSession.url) : PassiveCheckResult.CreateFailure(this, fiddlerSession.url);
        }
    }
}