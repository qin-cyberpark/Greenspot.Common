using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Greenspot.Identity.MySQL
{
    public class RoleRepository<TRole> where TRole : IdentityRole
    {
        private MySqlDatabase _database;

        public RoleRepository(MySqlDatabase database)
        {
            _database = database;
        }

        public IQueryable<TRole> GetRoles()
        {
            var roles = new List<TRole>();
            var rows = _database.Query(@"SELECT Id,Name FROM greenspot_roles", null);

            foreach (var row in rows)
            {
                var role = (TRole)Activator.CreateInstance(typeof(TRole));
                role.Id = row["Id"];
                role.Name = row["Name"];
                roles.Add(role);
            }

            return roles.AsQueryable();
        }

        public int Insert(IdentityRole role)
        {
            return _database.ExecuteNonQuery(@"INSERT INTO greenspot_roles (Id, Name) VALUES (@id,@name)",
                                            new Dictionary<string, object>{
                                                {"@name", role.Name},
                                                {"@id", role.Id}
                                            });
        }

        public int Delete(string roleId)
        {
            return _database.ExecuteNonQuery(@"DELETE FROM greenspot_roles WHERE Id = @id",
                                            new Dictionary<string, object>{
                                                {"@id", roleId}
                                            });
        }

        public IdentityRole GetRoleById(string roleId)
        {
            var roleName = GetRoleName(roleId);
            IdentityRole role = null;

            if (roleName != null)
            {
                role = new IdentityRole(roleName, roleId);
            }

            return role;

        }

        private string GetRoleName(string roleId)
        {
            return _database.GetStrValue(@"SELECT Name FROM greenspot_roles WHERE Id = @id",
                                        new Dictionary<string, object>{
                                            {"@id", roleId}
                                        });
        }

        public IdentityRole GetRoleByName(string roleName)
        {
            var roleId = GetRoleId(roleName);
            IdentityRole role = null;

            if (roleId != null)
            {
                role = new IdentityRole(roleName, roleId);
            }

            return role;
        }

        private string GetRoleId(string roleName)
        {

            return _database.GetStrValue(@"SELECT Id FROM greenspot_roles WHERE Name = @name",
                                        new Dictionary<string, object>(){
                                            {"@name", roleName}
                                        });
        }

        public int Update(IdentityRole role)
        {

            return _database.ExecuteNonQuery(@"UPDATE greenspot_roles SET Name = @name WHERE Id = @id",
                                            new Dictionary<string, object>{
                                                {"@name", role.Name},
                                                {"@id", role.Id}
                                            });
        }
    }
}
