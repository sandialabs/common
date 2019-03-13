using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// Not intended to be a 'serious' encryption scheme. It's just used to obfuscate
    /// things that need to be obfuscated without being too simple (like an XOR mask would be).
    /// </summary>
    public class SimpleEncryptDecrypt
    {
        public SimpleEncryptDecrypt()
        {
        }

        public string Encrypt(string str, Action<string> output = null)
        {
            string cypher = null;
            try
            {
                output?.Invoke("Encrypting: " + str);
                byte[] plaintext = Encoding.UTF8.GetBytes(str);

                using (RijndaelManaged rm = CreateRM())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, rm.CreateEncryptor(c_key, c_iv), CryptoStreamMode.Write))
                {
                    cs.Write(plaintext, 0, plaintext.Length);
                    cs.FlushFinalBlock();
                    byte[] ciphertext = ms.ToArray();
                    output?.Invoke("Ciphertext: " + BitConverter.ToString(ciphertext));
                    cypher = Convert.ToBase64String(ciphertext);
                    output?.Invoke("Base64 ciphertext: " + cypher);
                }
            }
            catch (Exception)
            {
            }
            return cypher;
        }

        public string Decrypt(string str, Action<string> output = null)
        {
            string plain = null;
            try
            {
                output?.Invoke("Decrypting: " + str);
                byte[] cypher = Convert.FromBase64String(str);
                output?.Invoke("Base64 cyphertext: " + BitConverter.ToString(cypher));
                using (RijndaelManaged rm = CreateRM())
                using (MemoryStream ms = new MemoryStream(cypher))
                using (CryptoStream cs = new CryptoStream(ms, rm.CreateDecryptor(c_key, c_iv), CryptoStreamMode.Read))
                {
                    byte[] plain_bytes = new byte[cypher.Length];
                    int length = cs.Read(plain_bytes, 0, plain_bytes.Length);
                    plain = Encoding.UTF8.GetString(plain_bytes, 0, length).Trim('\0');
                }
            }
            catch(FormatException)
            {
                // If it's an invalid base64 string, assume it's unencrypted and
                // return the original string
                plain = str;
                output?.Invoke("FormatException");
            }
            catch(CryptographicException)
            {
                // The string was able to be base-64 decoded, but isn't a valid encryped string.
                // Just return the original string
                plain = str;
                output?.Invoke("CryptographicException");
            }
            catch (Exception)
            {
                output?.Invoke("Exception");
            }

            output?.Invoke("Plaintext: " + plain);

            return plain;
        }

        private static RijndaelManaged CreateRM()
        {
            return new RijndaelManaged() { Padding = PaddingMode.Zeros };
        }

        private static readonly byte[] c_key = {
            0xFF, 0x61, 0xE5, 0x43, 0xBA, 0x7D, 0x54, 0x9D,
            0x81, 0x06, 0x82, 0xFB, 0x7E, 0x09, 0x5D, 0xF7,
            0x53, 0x26, 0xA6, 0xFD, 0x0A, 0xA6, 0x97, 0xBA,
            0xA6, 0x93, 0xC7, 0xA8, 0x97, 0x33, 0xF9, 0x09 };
        private static readonly byte[] c_iv = {
            0xA1, 0x36, 0x95, 0x4A, 0xA4, 0xC3, 0xA0, 0x12,
            0x2A, 0x96, 0xBD, 0x39, 0xF5, 0x50, 0x54, 0x91 };
    }
}
