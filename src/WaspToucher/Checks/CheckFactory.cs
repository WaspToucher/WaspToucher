using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace WaspToucher.Checks
{
    public class CheckFactory
    {
        private static readonly ILogger logger = ServiceLocator.Current.GetInstance<ILogger>();

        public static IEnumerable<IPassiveCheck> GetPassiveChecks(ComplianceStandard standard)
        {
            IEnumerable<IPassiveCheck> checkList = ServiceLocator.Current.GetAllInstances<IPassiveCheck>();
            checkList = checkList.Where(v => v.Compliances.Contains(standard)).AsEnumerable();

            logger.Trace("Got {0} passive checks for standard {1}:", checkList.Count(), standard.ToString());
            checkList.ForEach(v => logger.Trace("\t- {0}", v.Name));

            return checkList;
        }
    }
}