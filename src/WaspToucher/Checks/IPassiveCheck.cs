using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;

namespace WaspToucher.Checks
{
    public interface IPassiveCheck
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the compliances.
        /// </summary>
        ComplianceStandard[] Compliances { get; }

        /// <summary>
        /// Gets the information URL.
        /// </summary>
        Uri InformationUrl { get; }

        /// <summary>
        /// Runs the check.
        /// </summary>
        /// <param name="fiddlerSession">The fiddler session.</param>
        /// <returns>The result</returns>
        PassiveCheckResult RunCheck(Session fiddlerSession);
    }
}