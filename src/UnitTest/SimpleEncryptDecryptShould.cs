using gov.sandia.sld.common.utilities;
using System;
using Xunit;

namespace UnitTest
{
    public class SimpleEncryptDecryptShould
    {
        [Fact]
        public void ProperlyEncryptThenDecryptToTheSameString()
        {
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();

            string t1 = "abcd1234";
            string t1_cypher = sed.Encrypt(t1);
            Assert.NotEqual(t1, t1_cypher);
            string t1_plain = sed.Decrypt(t1_cypher);
            Assert.Equal(t1, t1_plain);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("1")]
        [InlineData("#")]
        [InlineData("@")]
        [InlineData("!")]
        [InlineData("1234567890")]
        [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890")]
        public void ProperlyEncryptAndDecryptStringsOfDifferentSizes(string plain)
        {
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
            string e = sed.Encrypt(plain);
            Assert.NotEqual(plain, e);
            string d = sed.Decrypt(e);
            Assert.Equal(plain, d);
        }

        [Fact]
        public void NotEncryptEmptyStrings()
        {
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
            string t1 = "";
            string t1_cypher = sed.Encrypt(t1);
            Assert.Equal(t1, t1_cypher);
        }

        [Fact]
        public void ProperlyEncryptThenDecryptGUIDs()
        {
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();

            for(int i = 0; i < 100; ++i)
            {
                string guid = Guid.NewGuid().ToString();
                string guid_e = sed.Encrypt(guid);
                Assert.NotEqual(guid, guid_e);
                string guid_d = sed.Decrypt(guid_e);
                Assert.Equal(guid, guid_d);
            }
        }

        [Fact]
        public void ReturnTheUnencryptedStringWhenDecryptingAnUnencryptedString()
        {
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();

            // Invalid base64 string--it should just return the original
            string unencrypted = "abc1234";
            string d = sed.Decrypt(unencrypted);
            Assert.Equal(unencrypted, d);

            unencrypted = "ChangeMe";
            d = sed.Decrypt(unencrypted);
            Assert.Equal(unencrypted, d);
        }
    }
}
