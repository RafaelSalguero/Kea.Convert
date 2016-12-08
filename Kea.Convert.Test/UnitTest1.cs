using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kea.Convert.Test
{
    [TestClass]
    public class ConvertTest
    {
        [TestMethod]
        public void CastToNullTest()
        {
            Assert.IsNull(ConvertHelper.Cast(typeof(object), null));
            Assert.IsNull(ConvertHelper.Cast(typeof(ConvertTest), null));
            Assert.IsNull(ConvertHelper.Cast(typeof(int?), null));
            Assert.IsNull(ConvertHelper.Cast(typeof(string), null));
        }

        [TestMethod]
        public void CastTest()
        {
            Assert.AreEqual(10M, ConvertHelper.Cast(typeof(decimal), 10));
            Assert.AreEqual(10, ConvertHelper.Cast(typeof(int), 10.7));
        }

        [TestMethod]
        public void ConvertFromStringTest()
        {
            Assert.AreEqual(DateTime.Parse("2016-01-26T00:00:00Z"), ConvertHelper.ConvertFromString("2016-01-26T00:00:00Z", typeof(DateTime)));
            Assert.AreEqual(123, ConvertHelper.ConvertFromString("123", typeof(int)));
            Assert.AreEqual(123.7, ConvertHelper.ConvertFromString("123.7", typeof(double)));

            var g = new Guid();
            Assert.AreEqual(g, ConvertHelper.ConvertFromString(g.ToString(), typeof(Guid)));
            Assert.AreEqual(null, ConvertHelper.ConvertFromString("", typeof(int?)));
            Assert.AreEqual(null, ConvertHelper.ConvertFromString(null, typeof(int?)));
            Assert.AreEqual(123, ConvertHelper.ConvertFromString("123", typeof(int?)));
            Assert.AreEqual("hello", ConvertHelper.ConvertFromString("hello", typeof(string)));
            Assert.AreEqual("", ConvertHelper.ConvertFromString("", typeof(string)));
            Assert.AreEqual(null, ConvertHelper.ConvertFromString(null, typeof(string)));
        }
    }
}
