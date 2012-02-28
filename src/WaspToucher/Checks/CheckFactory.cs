using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace WaspToucher.Checks
{
    public class CheckFactory
    {
        public static IEnumerable<IPassiveCheck> GetPassiveChecks(ComplianceStandard standard)
        {
            IEnumerable<IPassiveCheck> checkList = ServiceLocator.Current.GetAllInstances<IPassiveCheck>();
            return checkList.Where(v => v.Compliances.Contains(standard)).AsEnumerable();
        }
    }
}