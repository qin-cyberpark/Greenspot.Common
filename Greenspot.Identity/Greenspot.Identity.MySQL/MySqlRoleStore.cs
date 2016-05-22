using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Greenspot.Identity.MySQL
{
    public class MySqlRoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        public MySqlDatabase Database { get; private set; }
        private readonly RoleRepository<TRole> _roleRepository;

        public MySqlRoleStore()
        {
            _roleRepository = new RoleRepository<TRole>(new MySqlDatabase());
        }

        public MySqlRoleStore(MySqlDatabase database)
        {
            Database = database;
            _roleRepository = new RoleRepository<TRole>(database);
        }

        public IQueryable<TRole> Roles
        {
            get
            {
                return _roleRepository.GetRoles();
            }
        }

        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _roleRepository.Insert(role);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            _roleRepository.Delete(role.Id);

            return Task.FromResult<Object>(null);
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            var result = _roleRepository.GetRoleById(roleId) as TRole;

            return Task.FromResult(result);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            var result = _roleRepository.GetRoleByName(roleName) as TRole;
            return Task.FromResult(result);
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            _roleRepository.Update(role);

            return Task.FromResult<Object>(null);
        }

        public void Dispose()
        {
            if (Database != null)
            {
                Database.Dispose();
                Database = null;
            }
        }
    }
}
