using System.Collections.Generic;
using System.Data;
using Microsoft.AspNet.Identity;
using MySql.Data.MySqlClient;

namespace Greenspot.Identity.MySQL
{
    public class UserLoginRepository
    {
        private MySqlDatabase _database;
        public UserLoginRepository(MySqlDatabase database)
        {
            _database = database;
        }
    
        public int Insert(IdentityUser user, UserLoginInfo login)
        {
            return _database.ExecuteNonQuery(@"INSERT INTO greenspot_user_logins(UserId,LoginProvider,ProviderKey) VALUES(@Id,@LoginProvider,@ProviderKey)",
                                    new Dictionary<string, object>{
                                        {"@Id", user.Id},
                                        {"@LoginProvider", login.LoginProvider},
                                        {"@ProviderKey", login.ProviderKey}
                                   });
        }

        public int Delete(IdentityUser user, UserLoginInfo login)
        {
            return _database.ExecuteNonQuery(@"DELETE FROM greenspot_user_logins WHERE UserId = @Id AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey",
                                        new Dictionary<string, object>{
                                            {"@Id", user.Id},
                                            {"@LoginProvider", login.LoginProvider},
                                            {"@ProviderKey", login.ProviderKey}
                                       });
        }

        public string GetByUserLoginInfo(UserLoginInfo login)
        {
            return _database.GetStrValue(@"SELECT UserId FROM greenspot_user_logins WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey",
                                        new Dictionary<string, object>{
                                            {"@LoginProvider", login.LoginProvider},
                                            {"@ProviderKey", login.ProviderKey}
                                         });
        }

        public List<UserLoginInfo> PopulateLogins(string userId)
        {
            var listLogins = new List<UserLoginInfo>();

            var rows = _database.Query(@"SELECT LoginProvider,ProviderKey FROM greenspot_user_logins Where UserId = @Id",
                                            new Dictionary<string, object>{
                                                {"@Id",userId}
                                            });
            foreach(var row in rows)
            {
                listLogins.Add(new UserLoginInfo(row["LoginProvider"], row["ProviderKey"]));
            }
          
            return listLogins;
        }

   
    }
}
