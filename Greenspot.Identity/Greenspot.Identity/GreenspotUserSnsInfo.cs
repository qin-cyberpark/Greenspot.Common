using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Identity
{
    public class GreenspotUserSnsInfo
    {
        public GreenspotUserSnsInfo()
        {

        }
        public GreenspotUserSnsInfo(string snsName, string key, string value)
        {
            SnsName = snsName;
            InfoKey = key;
            InfoValue = value;
        }
        public string SnsName { get; set; }
        public string InfoKey { get; set; }
        public string InfoValue { get; set; }
    }
}
