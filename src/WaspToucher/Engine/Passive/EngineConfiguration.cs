namespace WaspToucher.Engine.Passive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WaspToucher.Checks;

    public class EngineConfiguration : IEngineConfiguration
    {
        /// <summary>
        /// Gets the proxy port number to listen on.
        /// </summary>
        public int ListenProxyPort
        {
            get
            {
                return 8877;
            }
        }

        /// <summary>
        /// Gets the compliance level to use.
        /// </summary>
        public ComplianceStandard Compliance
        {
            get
            {
                return ComplianceStandard.Owasp;
            }
        }
    }
}