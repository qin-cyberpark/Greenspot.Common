using Greenspot.Identity.MySQL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace Greenspot.Identity
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public partial class GreenspotUserManager : UserManager<GreenspotUser>
    {
        private bool _disposed;

        public GreenspotUserManager(IUserStore<GreenspotUser> store)
            : base(store)
        {
        }

        public static GreenspotUserManager Create(IdentityFactoryOptions<GreenspotUserManager> options, IOwinContext context)
        {
            var manager = new GreenspotUserManager(new MySqlUserStore<GreenspotUser>(context.Get<GreenspotIdentityDbContext>()));
            //var manager = new GreenspotUserManager(new MySqlUserStore<GreenspotUser>());

            // Configure validation logic for usernames
            manager.UserValidator = new GreenspotUserValidator(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<GreenspotUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<GreenspotUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<GreenspotUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        internal MySqlUserStore<GreenspotUser> GetMysqlUserStore()
        {
            var cast = Store as MySqlUserStore<GreenspotUser>;
            if (cast == null)
            {
                throw new NotSupportedException("not is a MySqlUserStore");
            }
            return cast;
        }

        public Task<GreenspotUser> FindByPhoneNumberAsync(string phone)
        {
            ThrowIfDisposed();
            var store = GetMysqlUserStore();
            if (string.IsNullOrEmpty(phone))
            {
                throw new ArgumentNullException("phone");
            }
            return store.FindByPhoneNumberAsync(phone);
        }

      

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     When disposing, actually dipose the store context
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                base.Dispose(disposing);
                _disposed = true;
            }
        }
    }
}
