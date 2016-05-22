// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using Greenspot.Identity.OAuth.WeChat;

namespace Owin
{
    /// <summary>
    /// Extension methods for using <see cref="WeChatAuthenticationMiddleware"/>
    /// </summary>
    public static class WeChatAuthenticationExtensions
    {
        /// <summary>
        /// Authenticate users using WeChat
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Middleware configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseWeChatAuthentication(this IAppBuilder app, WeChatAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(WeChatAuthenticationMiddleware), app, options);
            return app;
        }

        ///// <summary>
        ///// Authenticate users using WeChat
        ///// </summary>
        ///// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        ///// <param name="appId">The appId assigned by WeChat</param>
        ///// <param name="appSecret">The appSecret assigned by WeChat</param>
        ///// <returns>The updated <see cref="IAppBuilder"/></returns>
        //public static IAppBuilder UseWeChatAuthentication(
        //    this IAppBuilder app,
        //    string appId,
        //    string appSecret)
        //{
        //    return UseWeChatAuthentication(
        //        app,
        //        new WeChatAuthenticationOptions
        //        {
        //            AppId = appId,
        //            AppSecret = appSecret,
        //        });
        //}
    }
}
