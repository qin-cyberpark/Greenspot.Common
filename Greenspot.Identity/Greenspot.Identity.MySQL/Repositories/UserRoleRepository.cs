using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Greenspot.Identity.MySQL
{
    public class UserRoleRepository<TUser> where TUser:IdentityUser
    {
        private MySqlDatabase _database;
        public UserRoleRepository(MySqlDatabase database)
        {
            _database = database;
        }

        public int Insert(TUser user, string roleName)
        {
            var roleId =  _database.GetStrValue(@"SELECT Id FROM greenspot_roles WHERE Name=@RoleName", 
                                                new Dictionary<string, object>{
                                                    {"@RoleName", roleName}
                                                });
            if (!string.IsNullOrEmpty(roleId))
            {
                return _database.ExecuteNonQuery(@"Insert into greenspot_user_roles(UserId,RoleId) VALUES(@Id,@RoleId)"
                        , new Dictionary<string, object> {
                                                    {"@Id", user.Id},
                                                    {"@RoleId", roleId}});
            }

            return 0;
        }

        public int Delete(TUser user, string roleName)
        {
            var roleId = _database.GetStrValue(@"SELECT Id FROM greenspot_roles WHERE Name=@RoleName",
                                    new Dictionary<string, object>{
                                                    {"@RoleName", roleName}
                                    });

            if (!string.IsNullOrEmpty(roleId))
            {
                return _database.ExecuteNonQuery(@"Delete FROM greenspot_user_roles WHERE UserId=@Id AND RoleId=@RoleId"
                        , new Dictionary<string, object> {
                                                    {"@Id", user.Id},
                                                    {"@RoleId", roleId}});
            }

            return 0;
        }

        public List<string> PopulateRoles(string userId)
        {
            var roles = new List<string>();
            var rows = _database.Query(@"SELECT t2.* FROM greenspot_user_roles t1 INNER JOIN greenspot_roles t2 ON t1.RoleId = t2.Id WHERE t1.UserId = @Id",
                                        new Dictionary<string, object>
                                        {
                                            {"@Id", userId}
                                        }); 

            foreach(var row in rows)
            {
                roles.Add(row["Name"]);
            }

            return roles;
        }
    }
}
