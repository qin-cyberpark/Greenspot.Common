using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace Greenspot.Identity.MySQL
{
    public class UserRepository<TUser> where TUser : IdentityUser
    {
        private MySqlDatabase _database;
        public UserRepository(MySqlDatabase database)
        {
            _database = database;
        }

        public int Insert(TUser user)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"@Id", user.Id},
                    {"@Email", (object) user.Email ?? DBNull.Value},
                    {"@EmailConfirmed", user.EmailConfirmed},
                    {"@PasswordHash", (object) user.PasswordHash ?? DBNull.Value},
                    {"@SecurityStamp", (object) user.SecurityStamp ?? DBNull.Value},
                    {"@PhoneNumber", (object) user.PhoneNumber ?? DBNull.Value},
                    {"@PhoneNumberConfirmed", user.PhoneNumberConfirmed},
                    {"@TwoFactorAuthEnabled", user.TwoFactorAuthEnabled},
                    {"@LockoutEndDate", (object) user.LockoutEndDate ?? DBNull.Value},
                    {"@LockoutEnabled", user.LockoutEnabled},
                    {"@AccessFailedCount", user.AccessFailedCount},
                    {"@UserName", user.UserName}
                };

            return _database.ExecuteNonQuery(@"INSERT INTO greenspot_users VALUES(@Id,@Email,@EmailConfirmed,@PasswordHash,@SecurityStamp,@PhoneNumber,@PhoneNumberConfirmed,
                @TwoFactorAuthEnabled,@LockoutEndDate,@LockoutEnabled,@AccessFailedCount,@UserName)", parameters);
        }

        public int Delete(TUser user)
        {
            return _database.ExecuteNonQuery(@"DELETE FROM greenspot_users WHERE Id=@Id",
                                        new Dictionary<string, object>
                                        {
                                            {"@Id", user.Id}
                                        });
        }

        public IQueryable<TUser> GetAll()
        {
            List<TUser> users = new List<TUser>();

            var rows = _database.Query(@"SELECT * FROM greenspot_users", null);

            foreach (var row in rows)
            {
                users.Add(RowToUser(row));
            }

            return users.AsQueryable<TUser>();
        }

        public TUser GetById(string userId)
        {
           var rows = _database.Query(@"SELECT * FROM greenspot_users WHERE Id=@Id",
                                        new Dictionary<string, object>
                                        {
                                            {"@Id", userId}
                                        });

            foreach (var row in rows)
            {
                return RowToUser(row);
            }

            return null;
        }

        public TUser GetByName(string userName)
        {
            var rows = _database.Query(@"SELECT * FROM greenspot_users WHERE UserName=@UserName",
                                       new Dictionary<string, object>
                                       {
                                           {"@UserName", userName}
                                       });

            foreach (var row in rows)
            {
                return RowToUser(row);
            }

            return null;
        }

        public TUser GetByEmail(string email)
        {
            var rows = _database.Query(@"SELECT * FROM greenspot_users WHERE Email=@Email",
                                      new Dictionary<string, object>
                                      {
                                           {"@Email", email}
                                      });

            foreach (var row in rows)
            {
                return RowToUser(row);
            }

            return null;
        }

        public TUser GetByPhoneNumber(string phone)
        {
            var rows = _database.Query(@"SELECT * FROM greenspot_users WHERE PhoneNumber=@Phone",
                                      new Dictionary<string, object>
                                      {
                                           {"@Phone", phone}
                                      });

            foreach (var row in rows)
            {
                return RowToUser(row);
            }

            return null;
        }

        public int Update(TUser user)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"@NewId", user.Id},
                    {"@Email", (object) user.Email ?? DBNull.Value},
                    {"@EmailConfirmed", user.EmailConfirmed},
                    {"@PasswordHash", (object) user.PasswordHash ?? DBNull.Value},
                    {"@SecurityStamp", (object) user.SecurityStamp ?? DBNull.Value},
                    {"@PhoneNumber", (object) user.PhoneNumber ?? DBNull.Value},
                    {"@PhoneNumberConfirmed", user.PhoneNumberConfirmed},
                    {"@TwoFactorAuthEnabled", user.TwoFactorAuthEnabled},
                    {"@LockoutEndDate", (object) user.LockoutEndDate ?? DBNull.Value},
                    {"@LockoutEnabled", user.LockoutEnabled},
                    {"@AccessFailedCount", user.AccessFailedCount},
                    {"@UserName", user.UserName},
                    {"@Id", user.Id}
                };

            return _database.ExecuteNonQuery(@"UPDATE greenspot_users 
                SET Id = @NewId,Email=@Email,EmailConfirmed=@EmailConfirmed,PasswordHash=@PasswordHash,SecurityStamp=@SecurityStamp,PhoneNumber=@PhoneNumber,PhoneNumberConfirmed=@PhoneNumberConfirmed,
                TwoFactorEnabled=@TwoFactorAuthEnabled,LockoutEndDateUtc=@LockoutEndDate,LockoutEnabled=@LockoutEnabled,AccessFailedCount=@AccessFailedCount,UserName=@UserName
                WHERE Id=@Id", parameters);
        }

        private static TUser RowToUser(Dictionary<string, string> row)
        {
            var user = (TUser)Activator.CreateInstance(typeof(TUser));
            user.Id = row["Id"];
            user.Email = row["Email"];
            user.EmailConfirmed = "1".Equals(row["EmailConfirmed"]) ? true : false;
            user.PasswordHash = row["PasswordHash"];
            user.SecurityStamp = row["SecurityStamp"];
            user.PhoneNumber = row["PhoneNumber"];
            user.PhoneNumberConfirmed = "1".Equals(row["PhoneNumberConfirmed"]) ? true : false;
            user.TwoFactorAuthEnabled = "1".Equals(row["TwoFactorEnabled"]) ? true : false;
            user.LockoutEndDate = string.IsNullOrEmpty(row["LockoutEndDateUtc"]) ? (DateTime?)null : DateTime.Parse(row["LockoutEndDateUtc"]);
            user.LockoutEnabled = "1".Equals(row["LockoutEnabled"]) ? true : false;
            user.AccessFailedCount = string.IsNullOrEmpty(row["AccessFailedCount"]) ? 0 : int.Parse(row["AccessFailedCount"]);
            user.UserName = row["UserName"];
            return user;
        }

    }
}
