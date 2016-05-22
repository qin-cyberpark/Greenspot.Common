using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNet.Identity;
using Greenspot.Identity;
using Greenspot.Identity.MySQL;

namespace Greenspot.Common.Test
{
    [TestClass]
    public class IdentityTest
    {
        private GreenspotIdentityDbContext _context;
        private GreenspotSignInManager _signInManager;
        private GreenspotUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;

        [TestInitialize()]
        public void Initialize()
        {
            _context = GreenspotIdentityDbContext.Create();
            _userManager = new GreenspotUserManager(new MySqlUserStore<GreenspotUser>(_context));
            _roleManager = new RoleManager<IdentityRole>(new MySqlRoleStore<IdentityRole>(_context));
        }

        [TestMethod]
        public void CreateRole()
        {
            string rName = "RoleNameA";
            Assert.IsTrue(_roleManager.Create(new IdentityRole(rName)).Succeeded);

            var role = _roleManager.FindByName(rName);
            Assert.AreEqual(rName, role.Name);

            _roleManager.Delete(role);
            role = _roleManager.FindByName(rName);
            Assert.IsNull(role);
        }
    }
}
