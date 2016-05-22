using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Greenspot.Identity
{
    // Configure the application sign-in manager which is used in this application.
    public class GreenspotSignInManager : SignInManager<GreenspotUser, string>
    {
        public GreenspotSignInManager(GreenspotUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(GreenspotUser user)
        {
            return base.CreateUserIdentityAsync(user);
        }

        public static GreenspotSignInManager Create(IdentityFactoryOptions<GreenspotSignInManager> options, IOwinContext context)
        {
            return new GreenspotSignInManager(context.GetUserManager<GreenspotUserManager>(), context.Authentication);
        }
    }
}
