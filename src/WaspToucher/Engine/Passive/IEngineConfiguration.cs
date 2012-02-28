using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaspToucher.Checks;

namespace WaspToucher.Engine.Passive
{
    public interface IEngineConfiguration
    {
        /// <summary>
        /// Gets the proxy port number to listen on.
        /// </summary>
        int ListenProxyPort { get; }

        /// <summary>
        /// Gets the compliance level to use.
        /// </summary>
        ComplianceStandard Compliance { get; }
    }
}