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

            IEngine engine = ServiceLocator.Current.GetInstance<IEngine>();
            try
            {
                engine.Start();

                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
            finally
            {
                engine.Stop();
            }
        }
    }
}