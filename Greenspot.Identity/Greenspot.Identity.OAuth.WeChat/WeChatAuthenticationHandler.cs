// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Helpers;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Newtonsoft.Json.Linq;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.User;

namespace Greenspot.Identity.OAuth.WeChat
{
    internal partial class WeChatAuthenticationHandler : AuthenticationHandler<WeChatAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public WeChatAuthenticationHandler(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = null;

            try
            {
                #region code + token
                string code = null;
                string state = null;

                IReadableStringCollection query = Request.Query;

                IList<string> values = query.GetValues("code");
                if (values != null && values.Count == 1)
                {
                    code = values[0];
                }
                values = query.GetValues("state");
                if (values != null && values.Count == 1)
                {
                    var stateKey = values[0];
                    //load state
                    state = StateKeeper.Pop(stateKey);
                }

                properties = Options.StateDataFormat.Unprotect(state);
                if (properties == null)
                {
                    return null;
                }

                // OAuth2 10.12 CSRF
                if (!ValidateCorrelationId(properties, _logger))
                {
                    return new AuthenticationTicket(null, properties);
                }

                if (code == null)
                {
                    // Null if the remote server returns an error.
                    return new AuthenticationTicket(null, properties);
                }

                var tokenResult = OAuthApi.GetAccessToken(Uri.EscapeDataString(Options.AppId),
                                                                        Uri.EscapeDataString(Options.AppSecret),
                                                                        Uri.EscapeDataString(code));
                if (tokenResult == null || string.IsNullOrEmpty(tokenResult.access_token))
                {
                    _logger.WriteWarning("Access token was not found");
                    return new AuthenticationTicket(null, properties);
                }
                #endregion

                #region pull user info
                UserInfoJson userInfo = null;
                OAuthUserInfo oauthUserInfo = null;
                WeChatAuthenticatedContext context;

                if (Options.ScopeType == ScopeTypes.UserInfo)
                {
                    oauthUserInfo = OAuthApi.GetUserInfo(Options.AppId, tokenResult.openid, Senparc.Weixin.Language.zh_CN);
                }
                else if (Options.IsLoadUserInfo || Options.UseUnionIdAsIdentity)
                {
                    userInfo = UserApi.Info(Options.AppId, tokenResult.openid);
                }

                context = new WeChatAuthenticatedContext(Context, userInfo, oauthUserInfo,
                                                        tokenResult.access_token, tokenResult.expires_in);
                context.Identity = new ClaimsIdentity(Options.AuthenticationType, ClaimsIdentity.DefaultNameClaimType,
                                                         ClaimsIdentity.DefaultRoleClaimType);

                //identify by unionid or openid
                if (Options.UseUnionIdAsIdentity)
                {
                    //union id
                    context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userInfo?.unionid ?? oauthUserInfo?.unionid,
                        ClaimValueTypes.String, Options.AuthenticationType));
                }
                else
                {
                    //open id
                    context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, tokenResult.openid,
                        ClaimValueTypes.String, Options.AuthenticationType));
                }

                //open id
                context.Identity.AddClaim(new Claim(WeChatClaimTypes.OpenId, tokenResult.openid,
                                            ClaimValueTypes.String, Options.AuthenticationType));

                //union id
                if (!string.IsNullOrEmpty(userInfo?.unionid ?? oauthUserInfo?.unionid))
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.UnionId, userInfo?.unionid ?? oauthUserInfo?.unionid,
                                ClaimValueTypes.String, Options.AuthenticationType));
                }

                //nickname
                if (!string.IsNullOrEmpty(userInfo?.nickname ?? oauthUserInfo?.nickname))
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.NickName, userInfo?.nickname ?? oauthUserInfo?.nickname,
                                ClaimValueTypes.String, Options.AuthenticationType));
                }

                //city
                if (!string.IsNullOrEmpty(userInfo?.city ?? oauthUserInfo?.city))
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.City, userInfo?.city ?? oauthUserInfo?.city,
                        ClaimValueTypes.String, Options.AuthenticationType));
                }

                //province
                if (!string.IsNullOrEmpty(userInfo?.province ?? oauthUserInfo?.province))
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.Province, userInfo.province,
                        ClaimValueTypes.String, Options.AuthenticationType));
                }

                //country
                if (!string.IsNullOrEmpty(userInfo?.country ?? oauthUserInfo?.country))
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.Country, userInfo?.country ?? oauthUserInfo?.country,
                        ClaimValueTypes.String, Options.AuthenticationType));
                }

                //headimage
                if (!string.IsNullOrEmpty(userInfo?.headimgurl ?? oauthUserInfo?.headimgurl))
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.HeadImageUrl, userInfo?.headimgurl ?? oauthUserInfo?.headimgurl,
                        ClaimValueTypes.String, Options.AuthenticationType));
                }

                //sex
                if ((userInfo?.sex ?? 0) > 0)
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.Sex, userInfo.sex == 1 ? "M" : "F",
                        ClaimValueTypes.String, Options.AuthenticationType));
                }
                else if ((oauthUserInfo?.sex ?? 0) > 0)
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.Sex, oauthUserInfo.sex == 1 ? "M" : "F",
                       ClaimValueTypes.String, Options.AuthenticationType));
                }

                //privilege
                if (oauthUserInfo?.privilege != null && oauthUserInfo.privilege.Length > 0)
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.privilege, string.Join(",", oauthUserInfo.privilege),
                        ClaimValueTypes.String, Options.AuthenticationType));
                }

                //group
                if (userInfo?.groupid != null)
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.GroupId, userInfo.groupid.ToString(),
                        ClaimValueTypes.Integer32, Options.AuthenticationType));
                }

                //subscribed
                if (userInfo?.subscribe != null)
                {
                    context.Identity.AddClaim(new Claim(WeChatClaimTypes.Subscribed, userInfo.subscribe == 0 ? "False" : "True",
                        ClaimValueTypes.Boolean, Options.AuthenticationType));

                    if (userInfo.subscribe == 1) 
                    {
                        //subscribed time
                        if (userInfo?.subscribe_time != null)
                        {
                            context.Identity.AddClaim(new Claim(WeChatClaimTypes.SubscribedTime, DateTime.Now.AddMilliseconds(-userInfo.subscribe_time).ToString(),
                                ClaimValueTypes.DateTime, Options.AuthenticationType));
                        }
                    }
                }

                //loaded time
                context.Identity.AddClaim(new Claim(WeChatClaimTypes.LoadedTime, DateTime.Now.ToString(),
                                ClaimValueTypes.DateTime, Options.AuthenticationType));


                context.Properties = properties;
                #endregion

                await Options.Provider.Authenticated(context);

                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                _logger.WriteError("Authentication failed", ex);
                return new AuthenticationTicket(null, properties);
            }
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return Task.FromResult<object>(null);
            }

            AuthenticationResponseChallenge challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if (challenge != null)
            {
                string baseUri =
                    Request.Scheme +
                    Uri.SchemeDelimiter +
                    Request.Host +
                    Request.PathBase;

                string currentUri =
                    baseUri +
                    Request.Path +
                    Request.QueryString;

                string redirectUri = string.IsNullOrEmpty(Options.UnionCallbackPath) ? baseUri : Options.UnionCallbackPath
                                        + Options.CallbackPath;

                AuthenticationProperties properties = challenge.Properties;
                if (string.IsNullOrEmpty(properties.RedirectUri))
                {
                    properties.RedirectUri = currentUri;
                }

                // OAuth2 10.12 CSRF
                GenerateCorrelationId(properties);

                // comma separated
                string scope = string.Join(",", Options.Scope);

                string state = Options.StateDataFormat.Protect(properties);

                //store state
                var stateKey = StateKeeper.Put(state);

                string authorizationEndpoint =
                    Options.AuthorizationEndpoint +
                        "?appid=" + Uri.EscapeDataString(Options.AppId ?? string.Empty) +
                        "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                        "&scope=" + Uri.EscapeDataString(scope) +
                        "&response_type=code&state=" + Uri.EscapeDataString(stateKey) +
                        "#wechat_redirect";

                var redirectContext = new WeChatApplyRedirectContext(
                    Context, Options,
                    properties, authorizationEndpoint);
                Options.Provider.ApplyRedirect(redirectContext);
            }

            return Task.FromResult<object>(null);
        }

        public override async Task<bool> InvokeAsync()
        {
            return await InvokeReplyPathAsync();
        }

        private async Task<bool> InvokeReplyPathAsync()
        {
            if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
            {
                // TODO: error responses

                AuthenticationTicket ticket = await AuthenticateAsync();
                if (ticket == null)
                {
                    _logger.WriteWarning("Invalid return state, unable to redirect.");
                    Response.StatusCode = 500;
                    return true;
                }

                var context = new WeChatReturnEndpointContext(Context, ticket);
                context.SignInAsAuthenticationType = Options.SignInAsAuthenticationType;
                context.RedirectUri = ticket.Properties.RedirectUri;

                await Options.Provider.ReturnEndpoint(context);

                if (context.SignInAsAuthenticationType != null &&
                    context.Identity != null)
                {
                    ClaimsIdentity grantIdentity = context.Identity;
                    if (!string.Equals(grantIdentity.AuthenticationType, context.SignInAsAuthenticationType, StringComparison.Ordinal))
                    {
                        grantIdentity = new ClaimsIdentity(grantIdentity.Claims, context.SignInAsAuthenticationType, 
                                                            grantIdentity.NameClaimType, grantIdentity.RoleClaimType);
                    }
                    Context.Authentication.SignIn(context.Properties, grantIdentity);
                }

                if (!context.IsRequestCompleted && context.RedirectUri != null)
                {
                    string redirectUri = context.RedirectUri;
                    if (context.Identity == null)
                    {
                        // add a redirect hint that sign-in failed in some way
                        redirectUri = WebUtilities.AddQueryString(redirectUri, "error", "access_denied");
                    }
                    Response.Redirect(redirectUri);
                    context.RequestCompleted();
                }

                return context.IsRequestCompleted;
            }
            return false;
        }
    }
}
