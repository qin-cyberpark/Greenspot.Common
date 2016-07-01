using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Greenspot.Comm.SuburbMatrix.Models;

namespace Greenspot.Common.Test
{
    [TestClass]
    public class SuburbMatrixTest
    {
        [TestMethod]
        public void TestGoogleDirection()
        {
            SuburbMatrix matrix = new SuburbMatrix();
            var kms = matrix.GetDrivingDistance("Auckland Central, Auckland", "New Windsor, Auckland");
        }
    }
}
