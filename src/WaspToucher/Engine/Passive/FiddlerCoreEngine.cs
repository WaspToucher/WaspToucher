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
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiddlerCoreEngine"/> class.
        /// </summary>
        /// <param name="engineConfiguration">The engine configuration.</param>
        public FiddlerCoreEngine(ILogger engineLogger, IEngineConfiguration engineConfiguration)
        {
            configuration = engineConfiguration;
            logger = engineLogger;
        }

        public void Start()
        {
            if (Fiddler.FiddlerApplication.IsStarted())
            {
                throw new WaspToucherException("Passive listener has already started");
            }

            checkList = CheckFactory.GetPassiveChecks(configuration.Compliance);

            Fiddler.FiddlerApplication.Log.OnLogString += new EventHandler<Fiddler.LogEventArgs>(Log_OnLogString);
            Fiddler.FiddlerApplication.OnNotification += new EventHandler<Fiddler.NotificationEventArgs>(FiddlerApplication_OnNotification);
            Fiddler.FiddlerApplication.AfterSessionComplete += new Fiddler.SessionStateHandler(FiddlerApplication_AfterSessionComplete);
            Fiddler.FiddlerApplication.Startup(configuration.ListenProxyPort, true, true, true);

            logger.Info("Proxy listening on port {0}", configuration.ListenProxyPort);
        }

        private void FiddlerApplication_OnNotification(object sender, Fiddler.NotificationEventArgs e)
        {
            logger.Warn("Fiddler says: {0}", e.NotifyString);
        }

        private void Log_OnLogString(object sender, Fiddler.LogEventArgs e)
        {
            logger.Trace("Fiddler says: {0}", e.LogString);
        }

        /// <summary>
        /// Ellipsizes the specified text so that it will reduce the text to the specified length and appends "...".
        /// </summary>
        /// <example>
        /// "http://www.JustAnotherWebsite.com/AnotherPage.html" becomes "http://www.JustAnotherWeb..."
        /// </example>
        /// <param name="text">The text.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <returns>The ellipsized text</returns>
        private static string Ellipsize(string text, int maximumLength)
        {
            if (text.Length <= maximumLength) return text;
            return text.Substring(0, maximumLength - 3) + "...";
        }

        private void FiddlerApplication_AfterSessionComplete(Fiddler.Session oSession)
        {
            logger.Trace("{0} {1} {2}\n{3} {4} {5}\n\n", oSession.id, oSession.oRequest.headers.HTTPMethod, Ellipsize(oSession.fullUrl, 60), oSession.responseCode, oSession.GetResponseContentType(), oSession.ResponseBody.Length);

            checkList.ForEach(v =>
                {
                    try
                    {
                        logger.Trace("Running audit: " + v.Name);
                        PassiveCheckResult result = v.RunCheck(oSession);
                        if (!result.Passed)
                        {
                            logger.Warn("{0} failed for {1}", v.Name, oSession.fullUrl);
                        }

                        resultList.Add(result);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                });
        }

        public void Stop()
        {
            Fiddler.FiddlerApplication.Shutdown();
        }
    }
}