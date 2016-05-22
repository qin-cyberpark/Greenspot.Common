using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Identity
{
    public partial class GreenspotUserManager
    {
        public async Task<IdentityResult> CreateAsync(ExternalLoginInfo extInfo, GreenspotUser user)
        {
            ThrowIfDisposed();
            user.Logins.Add(extInfo.Login);
            var rslt = await CreateAsync(user).ConfigureAwait(false);
            if (rslt.Succeeded)
            {
                rslt = await AddLoginAsync(user.Id, extInfo.Login);
            }

            if (rslt.Succeeded)
            {
                var store = GetMysqlUserStore();
                //delete all
                await store.DeleteSnsInfoAsync(user);

                //store info
                foreach (var c in extInfo.ExternalIdentity.Claims)
                {
                    await store.AddSnsInfoAsync(user, new GreenspotUserSnsInfo(
                            extInfo.Login.LoginProvider, c.Type, c.Value));
                }

                await store.AddSnsInfoAsync(user, new GreenspotUserSnsInfo(
                        extInfo.Login.LoginProvider,
                        GreenspotUserSnsInfo.LAST_UPDATED_TIME,
                        DateTime.Now.ToLongTimeString()));
            }

            return rslt;
        }
    }
}
