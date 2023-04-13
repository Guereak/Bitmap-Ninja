using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BMP_App_WPF;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestConvertirEndianToInt()
        {
            byte[] bytes = { 0x01, 0x23, 0x45, 0x67 };
            int expectedValue = 0x67452301;

            int result = Convertir_Endian_To_Int(bytes);

            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void TestConvertirIntToEndian()
        {
            int value = 0x67452301;
            byte[] expectedBytes = { 0x01, 0x23, 0x45, 0x67 };

            byte[] result = Convertir_Int_To_Endian(value, 4);

            Assert.AreEqual(expectedBytes, result);
        }
    }
}
