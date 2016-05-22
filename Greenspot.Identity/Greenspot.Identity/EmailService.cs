using System;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace Greenspot.Identity
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return Task.FromResult(0);
        }
    }
}
