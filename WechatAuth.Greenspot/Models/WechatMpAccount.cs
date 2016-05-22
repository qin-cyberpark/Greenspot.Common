using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.WeChatAuth
{
    public class WeChatMpAccount
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("appSecret")]
        public string AppSecret { get; set; }

        [JsonProperty("applications")]
        public IList<Application> Applications { get; set; }
    }

}