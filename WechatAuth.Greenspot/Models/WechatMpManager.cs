using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;

namespace Greenspot.WeChatAuth
{
    public class WeChatMpManager
    {
        private static IList<WeChatMpAccount> _accounts;
        private static SortedList<string, Application> _applications;
        private static string _accountFilePath = ConfigurationManager.AppSettings["WeChatMpAccoutFilePath"];

        static WeChatMpManager()
        {
            Load();
        }

        public static void Load()
        {
            if (string.IsNullOrEmpty(_accountFilePath) || !File.Exists(_accountFilePath))
            {
                return;
            }
            string jsonStr = null;
            using (var rdr = new StreamReader(_accountFilePath))
            {
                jsonStr = rdr.ReadToEnd();
            }
            _accounts = JsonConvert.DeserializeObject<List<WeChatMpAccount>>(jsonStr);
            
            Sort();
        }

        private static void Sort()
        {
            if(_accounts == null)
            {
                return;
            }
            _applications = new SortedList<string, Application>();
            foreach(var acc in _accounts)
            {
                foreach(var app in acc.Applications)
                {
                    app.MpAccountName = acc.Name;
                    app.MpAppId = acc.AppId;
                    _applications.Add(app.AppId, app);
                }
            }
        }

        public static Application Find(string id)
        {
            if(_applications!=null && _applications.ContainsKey(id))
            {
                return _applications[id];
            }
            else
            {
                return null;
            }
        }

        public static IList<WeChatMpAccount> Accounts
        {
            get
            {
                return _accounts;
            }
        }
    }
}