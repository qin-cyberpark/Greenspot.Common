// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using Senparc.Weixin.MP.CommonAPIs;

namespace Greenspot.Identity.OAuth.WeChat
{
    public enum ApplicationTypes { MP, Web }

    [Flags]
    public enum ScopeTypes { None, Base, Login, UserInfo = 4 }


    /// <summary>
    /// Configuration options for <see cref="WeChatAuthenticationMiddleware"/>
    /// </summary>
    public class WeChatAuthenticationOptions : AuthenticationOptions
    {
        //constants
        private const string SCOPE_BASE = "snsapi_base";
        private const string SCOPE_LOGIN = "snsapi_login";
        private const string SCOPE_USERINFO = "snsapi_userinfo";
        private const string AUTHENTICATION_ENDPOINT_MP = "https://open.weixin.qq.com/connect/oauth2/authorize";
        private const string AUTHENTICATION_ENDPOINT_WEB = "https://open.weixin.qq.com/connect/qrconnect";


        private const string CALLBACK_PATH = "/signin-WeChat";
        private const int CALLBACK_TIMEOUT_SECOUNDS = 60;

        /// <summary>
        /// Initializes a new <see cref="WeChatAuthenticationOptions"/>
        /// </summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
            MessageId = "Greenspot.Owin.Security.WeChat.WeChatAuthenticationOptions.set_Caption(System.String)",
            Justification = "Not localizable.")]
        public WeChatAuthenticationOptions(ApplicationTypes appType, ScopeTypes scopeTypes, string appId, string appSecret)
            : base(GetAuthenticationType(appType))

        {
            Caption = GetAuthenticationType(appType);
            CallbackPath = new PathString(CALLBACK_PATH);
            AuthenticationMode = AuthenticationMode.Passive;
            ScopeType = scopeTypes;
            BackchannelTimeout = TimeSpan.FromSeconds(CALLBACK_TIMEOUT_SECOUNDS);
            AuthorizationEndpoint = GetAuthorizationEndpoint(appType);
            AppId = appId;
            AppSecret = appSecret;

            AccessTokenContainer.Register(AppId, AppSecret);
        }

        /// <summary>
        /// Gets or sets the WeChat-assigned appId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the WeChat-assigned app secret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// Gets or sets the a pinned certificate validator to use to validate the endpoints used
        /// in back channel communications belong to WeChat.
        /// </summary>
        /// <value>
        /// The pinned certificate validator.
        /// </value>
        /// <remarks>If this property is null then the default certificate checks are performed,
        /// validating the subject name and if the signing chain is a trusted party.</remarks>
        public ICertificateValidator BackchannelCertificateValidator { get; set; }

        /// <summary>
        /// Gets or sets timeout value in milliseconds for back channel communications with WeChat.
        /// </summary>
        /// <value>
        /// The back channel timeout in milliseconds.
        /// </value>
        public TimeSpan BackchannelTimeout { get; set; }

        /// <summary>
        /// The HttpMessageHandler used to communicate with WeChat.
        /// This cannot be set at the same time as BackchannelCertificateValidator unless the value 
        /// can be downcast to a WebRequestHandler.
        /// </summary>
        public HttpMessageHandler BackchannelHttpHandler { get; set; }

        /// <summary>
        /// Get or sets the text that the user can display on a sign in user interface.
        /// </summary>
        public string Caption
        {
            get { return Description.Caption; }
            private set { Description.Caption = value; }
        }

        /// <summary>
        /// The request path within the application's base path where the user-agent will be returned.
        /// The middleware will process this request when it arrives.
        /// Default value is "/signin-WeChat".
        /// </summary>
        public PathString CallbackPath { get; private set; }
        public string UnionCallbackPath { get; set; }
        /// <summary>
        /// Gets or sets the name of another authentication middleware which will be responsible for actually issuing a user <see cref="System.Security.Claims.ClaimsIdentity"/>.
        /// </summary>
        public string SignInAsAuthenticationType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IWeChatAuthenticationProvider"/> used to handle authentication events.
        /// </summary>
        public IWeChatAuthenticationProvider Provider { get; set; }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

        /// <summary>
        /// A list of permissions to request.
        /// </summary>
        public string Scope { get { return GetScope(ScopeType); } }

        public ScopeTypes ScopeType { get; set; }

        /// <summary>
        /// Gets or sets the URI where the client will be redirected to authenticate.
        /// for MP default value is 'https://open.weixin.qq.com/connect/oauth2/authorize'.
        /// for Web default value is 'https://open.weixin.qq.com/connect/qrconnect'.
        /// </summary>
        public string AuthorizationEndpoint { get; set; }

        private static string GetAuthenticationType(ApplicationTypes appType)
        {
            switch (appType)
            {
                case ApplicationTypes.MP: return WeChatAuthenticationTypes.MP;
                case ApplicationTypes.Web: return WeChatAuthenticationTypes.Web;
                default: return WeChatAuthenticationTypes.MP;
            }
        }

        private static string GetAuthorizationEndpoint(ApplicationTypes appType)
        {
            switch (appType)
            {
                case ApplicationTypes.MP: return AUTHENTICATION_ENDPOINT_MP;
                case ApplicationTypes.Web: return AUTHENTICATION_ENDPOINT_WEB;
                default: return AUTHENTICATION_ENDPOINT_MP;
            }
        }

        private static string GetScope(ScopeTypes scopeTypes)
        {
            if (scopeTypes == ScopeTypes.None)
            {
                return null;
            }

            var scopes = new List<string>();
            if (scopeTypes.HasFlag(ScopeTypes.Base))
            {
                scopes.Add(SCOPE_BASE);
            }
            if (scopeTypes.HasFlag(ScopeTypes.Login))
            {
                scopes.Add(SCOPE_LOGIN);
            }
            if (scopeTypes.HasFlag(ScopeTypes.UserInfo))
            {
                scopes.Add(SCOPE_USERINFO);
            }

            return string.Join(",", scopes);
        }

    }
}
