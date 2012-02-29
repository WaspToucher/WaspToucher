using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaspToucher.Engine.Passive
{
    public interface IEngine
    {
        /// <summary>
        /// Starts the engine.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the engine.
        /// </summary>
        void Stop();
    }
}