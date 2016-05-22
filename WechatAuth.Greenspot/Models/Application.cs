using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.WeChatAuth
{
    public class Application
    {
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonIgnore]
        public string MpAccountName { get; set; }

        [JsonIgnore]
        public string MpAppId { get; set; }

        public string GetRedirectUrl(string url, string code, string state)
        {
            var result = string.Format("{0}/{1}?code={2}&state={3}", RedirectUrl.TrimEnd('/'), url.TrimStart('/'), code, state);
            return result;
        }
    }
   
}