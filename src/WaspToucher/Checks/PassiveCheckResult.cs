using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;

namespace WaspToucher.Checks
{
    public class PassiveCheckResult
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="PassiveCheckResult"/> class from being created.
        /// </summary>
        private PassiveCheckResult()
        { }

        /// <summary>
        /// Creates the result.
        /// </summary>
        /// <param name="passed">if set to <c>true</c> then the check passed.</param>
        /// <param name="check">The check.</param>
        /// <param name="url">The URL.</param>
        /// <returns>The result</returns>
        private static PassiveCheckResult CreateResult(bool passed, IPassiveCheck check, string url)
        {
            PassiveCheckResult result = new PassiveCheckResult();
            result.Passed = passed;
            result.Url = url;

            return result;
        }

        public static PassiveCheckResult CreateFailure(IPassiveCheck check, string url)
        {
            PassiveCheckResult result = CreateResult(false, check, url);

            return result;
        }

        public static PassiveCheckResult CreatePass(IPassiveCheck check, string url)
        {
            PassiveCheckResult result = CreateResult(true, check, url);

            return result;
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        public string Url
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PassiveCheckResult"/> has passed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if passed; otherwise, <c>false</c>.
        /// </value>
        public bool Passed
        {
            get;
            private set;
        }
    }
}