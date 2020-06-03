using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace Joint.Common
{
    public class SecureHelper
    {
        public static readonly string JeasuSecretKey = "JeasuSecretKey";
        public static readonly string JeasuAutoKey = Common.TextFilter.GetPasswordSalt();

        private static readonly byte[] _aeskeys = new byte[] { 0x12, 0x34, 0x56, 120, 0x90, 0xab, 0xcd, 0xef, 0x12, 0x34, 0x56, 120, 0x90, 0xab, 0xcd, 0xef };
        private static Regex _base64regex = new Regex(@"[A-Za-z0-9\=\/\+]");
        private static Regex _sqlkeywordregex = new Regex(@"(select|insert|delete|from|count\(|drop|table|update|truncate|asc\(|mid\(|char\(|xp_cmdshell|exec|master|net|local|group|administrators|user|or|and|-|;|,|\(|\)|\[|\]|\{|\}|%|@|\*|!|\')", RegexOptions.IgnoreCase);

        public static string AESDecrypt(string decryptStr, string decryptKey)
        {
            if (string.IsNullOrWhiteSpace(decryptStr))
            {
                return string.Empty;
            }
            decryptKey = StringHelper.SubString(decryptKey, 0x20);
            decryptKey = decryptKey.PadRight(0x20, ' ');
            byte[] buffer = Convert.FromBase64String(decryptStr);
            SymmetricAlgorithm algorithm = Rijndael.Create();
            algorithm.Key = Encoding.UTF8.GetBytes(decryptKey);
            algorithm.IV = _aeskeys;
            byte[] buffer2 = new byte[buffer.Length];
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (CryptoStream stream2 = new CryptoStream(stream, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    stream2.Read(buffer2, 0, buffer2.Length);
                    stream2.Close();
                    stream.Close();
                }
            }
            return Encoding.UTF8.GetString(buffer2).Replace("\0", "");
        }

        public static string AESEncrypt(string encryptStr, string encryptKey)
        {
            if (string.IsNullOrWhiteSpace(encryptStr))
            {
                return string.Empty;
            }
            encryptKey = StringHelper.SubString(encryptKey, 0x20);
            encryptKey = encryptKey.PadRight(0x20, ' ');
            SymmetricAlgorithm algorithm = Rijndael.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(encryptStr);
            algorithm.Key = Encoding.UTF8.GetBytes(encryptKey);
            algorithm.IV = _aeskeys;
            byte[] inArray = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream stream2 = new CryptoStream(stream, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    stream2.Write(bytes, 0, bytes.Length);
                    stream2.FlushFinalBlock();
                    inArray = stream.ToArray();
                    stream2.Close();
                    stream.Close();
                }
            }
            return Convert.ToBase64String(inArray);
        }

        public static string DecodeBase64(string result)
        {
            return DecodeBase64(Encoding.UTF8, result);
        }

        public static string DecodeBase64(Encoding encode, string result)
        {
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                return encode.GetString(bytes);
            }
            catch
            {
                return result;
            }
        }

        public static string EncodeBase64(string source)
        {
            return EncodeBase64(Encoding.UTF8, source);
        }

        public static string EncodeBase64(Encoding encode, string source)
        {
            return Convert.ToBase64String(encode.GetBytes(source));
        }

        public static bool IsBase64String(string str)
        {
            if (str != null)
            {
                return _base64regex.IsMatch(str);
            }
            return true;
        }

        public static bool IsSafeSqlString(string s)
        {
            if ((s != null) && _sqlkeywordregex.IsMatch(s))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 用MD5给数据加密
        /// </summary>
        /// <param name="str">要加密的数据</param>
        /// <returns>加密后的MD5值</returns>
        public static string MD5(string str)
        {
            //return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToUpper();

            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

    }
}
