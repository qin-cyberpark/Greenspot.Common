using Greenspot.Identity.MySQL;

namespace Greenspot.Identity
{
    public class GreenspotIdentityDbContext : MySqlDatabase
    {
        public GreenspotIdentityDbContext(string connectionName)
            : base(connectionName)
        {
        }

        public static GreenspotIdentityDbContext Create()
        {
            return new GreenspotIdentityDbContext("GreenspotIdentityConnection");
        }
    }
}
