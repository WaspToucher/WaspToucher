using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaspToucher.Checks;

namespace WaspToucher.Engine.Passive
{
    public class FiddlerCoreEngine : IEngine
    {
        private readonly IEngineConfiguration configuration;
        private IEnumerable<IPassiveCheck> checkList;
        private IList<PassiveCheckResult> resultList = new List<PassiveCheckResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FiddlerCoreEngine"/> class.
        /// </summary>
        /// <param name="engineConfiguration">The engine configuration.</param>
        public FiddlerCoreEngine(IEngineConfiguration engineConfiguration)
        {
            configuration = engineConfiguration;
        }

        public void Start()
        {
            if (Fiddler.FiddlerApplication.IsStarted())
            {
                throw new WaspToucherException("Passive listener has already started");
            }

            checkList = CheckFactory.GetPassiveChecks(configuration.Compliance);

            Fiddler.FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;
            Fiddler.FiddlerApplication.Startup(configuration.ListenProxyPort, true, true, true);
        }

        private void FiddlerApplication_AfterSessionComplete(Fiddler.Session oSession)
        {
            Console.WriteLine("Auditing " + oSession.url);

            checkList.ForEach(v => resultList.Add(v.RunCheck(oSession)));
        }

        public void Stop()
        {
            Fiddler.FiddlerApplication.Shutdown();
        }
    }
}