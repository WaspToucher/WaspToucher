using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using WaspToucher.Engine.Passive;

namespace WaspToucher.Runner
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BootStrapper.Initialise();

            ILogger logger = ServiceLocator.Current.GetInstance<ILogger>();
            IEngine engine = ServiceLocator.Current.GetInstance<IEngine>();

            try
            {
                engine.Start();

                logger.Info("Press enter to quit");
                Console.ReadLine();
                logger.Info("Quiting...");
            }
            finally
            {
                engine.Stop();
            }
        }
    }
}