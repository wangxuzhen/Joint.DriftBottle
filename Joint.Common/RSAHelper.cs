//引入命名空间
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;


namespace Joint.Common
{
    public class RSAHelper
    {
        //public static string keyPath = System.Web.HttpContext.Current.Server.MapPath("~/SiteFile");
        private static readonly string privateKey = "Q++Bw/JgaEssBhJfKJ2JLUm903Ro0iaAVHEkxQKvcKWOdwpK9iZExCkKpluizqXFzuzFlD9oHffgYwDBRiKL3HGe93WTB0inN3GazZKZ30oRtKnZik5IKECRIUqBm8oKBiniZZC+cazEWaCI5nbCky/87pNNlTKHMh/ctjPFFhOP/Z1vtMZM/Ho9iVeuL5hTVyuOUvIRbWrCADwZAk9pSir9hQrTqFdfJKeENgKXl1C8d9u3ol3Bl4A9UCVEFr8AdaxbwUiIOh2dOIcss4quQxBMAj8M4mI/LSuHrOV9bh92ULJYEzlXEiGyMT3JRbv3IvesgIlanL5JA1X5C1r75s426Pasb3YYollyyJP6hSMEsMveeglkJKvCj0NCG8OK3d/BuqPlSr+l+3jPdMc/L8BffjNQG0lpOZvOX1pPxKQK8TZdGj8ArLmhh7jiMMFUC3ZzG1EPUCThgDsqQ/gFAwAq8xFKaKwUFW83zI2w7OyxKglaxrhdlO4+SFvKL5xZySepLm0Y6EskB6EtoCE0IkqVWeN3ItWm+KrW1HSxMtNERYchp1ToRLiGkFH8iy2F1/Z5cPHQGOQ+qLN2H5nFuJn8VDlb2UZDIgrEDdOqVc3d4U4RlNwaBJbblgGJC84b/Vs2Qo7ILDiXCY32tN7W7Id3x1s9rT3vE+ffiXnv05gvReUxl+xhuzquxvXhwvhfyJw8wbjt1SdIy7NZ3+ynsTbzRq4xjzHmsRzVqd7n9QOq7ubNkJq9p8KtOuZK+I9PTo61YC0L8caJGkAb7PvEo2wwdp2PvPlloDyWjXarFh5sYfXyQfKeGvY/rarw9GdEBImOeHY2Tea4UBlm5FaiM8pw6pX3o+ssiItdJM2S9Ht0Xp7ad2z8z4l7ty4qtRkTGJZpHZ3HZsHWZxvE7AgjelZMUZlMNfUK9TFORwjNrzwNWROUyfYo9s2NrL6oDYldq/JGw23ZJU8Hjq5LDGVd25bwUcVJHx4QRT9s/xsiJr8VrMjcUWgGHM4YSrzFEIuK7YQddoBbAZ9TzaTbSGqBHvdX/ru/0baJ+UYQXFzCP0mDN/0H5ycou2m+zYjuwJskCG732aHl4ut8WmQM3e9A10LOXktzcjvD8oC/HP6cvmSbkNiUzpvjUe0MSisKnH1GHO+SemapeFdSdFfJ4E/10gWlYI2rXh7SF5nTJFC8BnLe3NWRLRbMmBMURy09qzIFtUjenpLtoOw34y4l8UxPTA==";

        /// <summary>
        /// 使用RSA实现解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <returns></returns>
        public static string RSADecrypt(string data)
        {
            //C#默认只能使用[私钥]进行解密(想使用[私钥加密]可使用第三方组件BouncyCastle来实现)
            //string privateKeyPath = keyPath + @"\PrivateKey.xml";
            //string privateKey = File.ReadAllText(privateKeyPath);
            //创建RSA对象并载入[私钥]
            RSACryptoServiceProvider rsaPrivate = new RSACryptoServiceProvider();
            rsaPrivate.FromXmlString(SecureHelper.AESDecrypt(privateKey, "JeasuSecretKey"));
            //对数据进行解密
            byte[] privateValue = rsaPrivate.Decrypt(Convert.FromBase64String(data), false);//使用Base64将string转换为byte
            string privateStr = Encoding.UTF8.GetString(privateValue);
            return privateStr;
        }
    }
}
