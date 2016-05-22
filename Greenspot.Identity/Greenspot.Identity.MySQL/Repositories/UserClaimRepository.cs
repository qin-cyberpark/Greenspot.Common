using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using MySql.Data.MySqlClient;

namespace Greenspot.Identity.MySQL
{
    public class UserClaimRepository<TUser> where TUser : IdentityUser
    {
        private MySqlDatabase _database;
        public UserClaimRepository(MySqlDatabase database)
        {
            _database = database;
        }

        public int Insert(TUser user, Claim claim)
        {
            return _database.ExecuteNonQuery(@"INSERT INTO greenspot_user_claims(UserId,ClaimType,ClaimValue) VALUES(@UserId,@ClaimType,@ClaimValue)",
                                     new Dictionary<string, object>{
                                                {"@UserId", user.Id},
                                                {"@ClaimType", claim.Type},
                                                {"@ClaimValue",claim.Value}
                                    });
        }

        public int Delete(TUser user, Claim claim)
        {
            return _database.ExecuteNonQuery(@"DELETE FROM greenspot_user_claims WHERE UserId=@UserId AND ClaimType=@ClaimType AND ClaimValue=@ClaimValue",
                                     new Dictionary<string, object>{
                                            {"@UserId", user.Id},
                                            {"@ClaimType", claim.Type},
                                            {"@ClaimValue", claim.Value}
                                     });
        }

        public List<IdentityUserClaim> PopulateClaims(string userId)
        {
            var claims = new List<IdentityUserClaim>();
            var rows = _database.Query(@"SELECT ClaimType,ClaimValue FROM greenspot_user_claims WHERE UserId=@Id",
                                         new Dictionary<string, object>{
                                            {"@Id", userId}
                                         });

            foreach (var row in rows)
            {
                claims.Add(new IdentityUserClaim() {
                    ClaimType = row["ClaimType"],
                    ClaimValue = row["ClaimValue"]
                });
            }

            return claims;
        }
    }
}
