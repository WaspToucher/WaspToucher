using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaspToucher
{
    public class WaspToucherException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaspToucherException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public WaspToucherException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaspToucherException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public WaspToucherException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}