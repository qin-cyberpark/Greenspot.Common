using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Greenspot.Identity
{
    /// <summary>
        ///     Validates users before they are saved to an IUserStore
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TKey"></typeparam>
    public class GreenspotUserValidator : IIdentityValidator<GreenspotUser>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="manager"></param>
        public GreenspotUserValidator(GreenspotUserManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            AllowOnlyAlphanumericUserNames = true;
            Manager = manager;
        }

        /// <summary>
                ///     Only allow [A-Za-z0-9@_] in UserNames
                /// </summary>
        public bool AllowOnlyAlphanumericUserNames { get; set; }

        /// <summary>
                ///     If set, enforces that emails are non empty, valid, and unique
                /// </summary>
        public bool RequireUniqueEmail { get; set; }

        private GreenspotUserManager Manager { get; set; }

        /// <summary>
                ///     Validates a user before saving
                /// </summary>
                /// <param name="item"></param>
                /// <returns></returns>
        public virtual async Task<IdentityResult> ValidateAsync(GreenspotUser item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var errors = new List<string>();

            //at least have one of email, phone, username or login
            if (string.IsNullOrEmpty(item.UserName) && string.IsNullOrEmpty(item.Email) &&
                string.IsNullOrEmpty(item.PhoneNumber) && item.Logins.Count == 0)
            {
                errors.Add("at least have one of email, phone, username or login");
            }

            //check username
            await ValidateUserName(item, errors);

            //check mail
            if (RequireUniqueEmail)
            {
                await ValidateEmail(item, errors);
            }

            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }

        private async Task ValidateUserName(GreenspotUser user, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                //ok for no userName
                return;
            }

            if (AllowOnlyAlphanumericUserNames && !Regex.IsMatch(user.UserName, @"^[A-Za-z0-9@_\.]+$"))
            {
                // If any characters are not letters or digits, its an illegal user name
                errors.Add(String.Format(CultureInfo.CurrentCulture,
                    @"User name '{0}' is invalid, can only contain A-Za-z0-9@_\.", user.UserName));
            }
            else
            {
                var owner = await Manager.FindByNameAsync(user.UserName);
                if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
                {
                    errors.Add(String.Format(CultureInfo.CurrentCulture,
                        "A user with username '{0}' already exists.", user.UserName));
                }
            }
        }

        // make sure email is valid, and unique
        private async Task ValidateEmail(GreenspotUser user, List<string> errors)
        {
            //var email = await Manager.GetEmailAsync(user.Id).ConfigureAwait(false);
            var email = user.Email;
            if (string.IsNullOrWhiteSpace(email))
            {
                //ok for no email
                return;
            }


            try
            {
                var m = new MailAddress(email);
            }
            catch (FormatException)
            {
                errors.Add(string.Format(CultureInfo.CurrentCulture,
                    @"email '{0}' is invalid", email));
                return;
            }
            var owner = await Manager.FindByEmailAsync(email);
            if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add(string.Format(CultureInfo.CurrentCulture,
                        "A user with email '{0}' already exists.", email));
            }
        }

        private async Task ValidatePhoneNumber(GreenspotUser user, List<string> errors)
        {
            var phone = user.PhoneNumber;
            if (string.IsNullOrWhiteSpace(phone))
            {
                //ok for no phone
                return;
            }

            if (!Regex.IsMatch(phone, @"^\d+$"))
            {
                // If any characters are not letters or digits, its an illegal user name
                errors.Add(String.Format(CultureInfo.CurrentCulture,
                    @"Phone Number '{0}' is invalid.", phone));
            }

            var owner = await Manager.FindByPhoneNumberAsync(phone);
            if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add(string.Format(CultureInfo.CurrentCulture,
                        "A user with Phone '{0}' already exists.", phone));
            }
        }
    }
}
