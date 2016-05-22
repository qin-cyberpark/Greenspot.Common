using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Identity.OAuth.WeChat
{
    public class WeChatAuthenticationTypes
    {
        public const string MP = "WeChatMP";
        public const string Web = "WeChatWeb";
    }
    public class WeChatClaimTypes
    {
        private WeChatClaimTypes() { }
        public const string UnionId = "greenspot:identity:claims:wechat:unionid";
        public const string OpenId = "greenspot:identity:claims:wechat:openid";
        public const string NickName = "greenspot:identity:claims:wechat:nickname";
        public const string City = "greenspot:identity:claims:wechat:city";
        public const string privilege = "greenspot:identity:claims:wechat:privilege";
        public const string Province = "greenspot:identity:claims:wechat:province";
        public const string Country = "greenspot:identity:claims:wechat:country";
        public const string HeadImageUrl = "greenspot:identity:claims:wechat:headimgurl";
        public const string Sex = "greenspot:identity:claims:wechat:sex";
    }
}