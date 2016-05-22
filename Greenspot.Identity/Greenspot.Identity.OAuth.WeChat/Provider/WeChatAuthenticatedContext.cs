// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;

namespace Greenspot.Identity.OAuth.WeChat
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class WeChatAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="WeChatAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="accessToken">WeChat Access token</param>
        /// <param name="expires">Seconds until expiration</param>
        public WeChatAuthenticatedContext(IOwinContext context, OAuthUserInfo userInfo, string accessToken, int expiresIn)
            : base(context)
        {
            UserInfo = userInfo;
            AccessToken = accessToken;
            ExpiresIn = TimeSpan.FromSeconds(expiresIn);
        }

        /// <summary>
        /// Gets the JSON-serialized user
        /// </summary>
        public OAuthUserInfo UserInfo { get; set; }

        /// <summary>
        /// Gets the WeChat access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the WeChat access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn { get; set; }


        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }
    }
}
