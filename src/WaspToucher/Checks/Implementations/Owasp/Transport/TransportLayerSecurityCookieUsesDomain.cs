namespace WaspToucher.Checks.Implementations.Owasp.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TransportLayerSecurityCookieUsesDomain : IPassiveCheck
    {
        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return "Cookie should be marked with domain whilst using TLS";
            }
        }

        /// <summary>
        /// Gets the compliances.
        /// </summary>
        public ComplianceStandard[] Compliances
        {
            get
            {
                return new ComplianceStandard[] { ComplianceStandard.Owasp };
            }
        }

        public Uri InformationUrl
        {
            get { throw new NotImplementedException(); }
        }

        public PassiveCheckResult RunCheck(Fiddler.Session fiddlerSession)
        {
            if (fiddlerSession.isHTTPS && fiddlerSession.oResponse.headers.Exists("set-cookie"))
            {
                string cookie = fiddlerSession.oResponse.headers["set-cookie"];

                if (cookie != null && cookie.Length > 0)
                {
                    string[] parts = cookie.Split(';');
                    string cookiename = parts[0];
                    cookiename = cookiename.Split('=')[0];

                    if (parts != null && parts.Length > 0)
                    {
                        bool isDomainSet = false;

                        parts.ForEach(v =>
                        {
                            if (v.Trim().ToLower().StartsWith("domain"))
                            {
                                isDomainSet = true;
                            }
                        });

                        if (!isDomainSet)
                        {
                            return PassiveCheckResult.CreateFailure(this, fiddlerSession.fullUrl, "Cookie not marked with domain");
                        }
                    }
                }
            }

            return PassiveCheckResult.CreatePass(this, fiddlerSession.fullUrl);
        }
    }
}