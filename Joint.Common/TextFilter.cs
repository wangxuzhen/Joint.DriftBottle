using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.IO;

namespace Joint.Common
{
    public class TextFilter
    {
        /// <summary>
        /// 过滤html字符串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Filter(string text)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            text = Regex.Replace(text, "[\\s]{2,}", "");    //two or more spaces
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");    //<br>
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");    //&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);    //any other tags
            text = text.Replace("&", "");
            return text;
        }


        /// <summary > 
        /// 过滤所有的Html标签 
        /// </summary > 
        /// <param name="Htmlstring" ></param > 
        /// <returns ></returns > 
        public static string RemoveHTML(string Htmlstring)
        {
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^ >]*? >.*?</script >", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^ >]*) >", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-- >", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase); Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", " >", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase); Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(" >", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }


        /// <summary>
        /// 过滤标记
        /// </summary>
        /// <param name="NoHTML">包括HTML，脚本，数据库关键字，特殊字符的源码 </param>
        /// <returns>已经去除标记后的文字</returns>
        public static string FilterHtml(string Htmlstring)
        {
            if (Htmlstring == null)
            {
                return "";
            }
            else
            {
                //过滤html,js,css代码
                Htmlstring = Regex.Replace(Htmlstring, @"<style[\s\S]*?</style\s*>", "", RegexOptions.IgnoreCase);
                //删除脚本
                Htmlstring = Regex.Replace(Htmlstring, @"<script[\s\S]*?</script\s*>", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

                Htmlstring = Regex.Replace(Htmlstring, "[\\s]{2,}", " "); //两个或多个空格替换为一个
                Htmlstring = Regex.Replace(Htmlstring, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n"); //<br>
                Htmlstring = Regex.Replace(Htmlstring, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " "); //&nbsp;
                Htmlstring = Regex.Replace(Htmlstring, "<(.|\\n)*?>", string.Empty); //其它任何标记




                //删除HTML
                Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "xp_cmdshell", "", RegexOptions.IgnoreCase);

                //删除与数据库相关的词
                Htmlstring = Regex.Replace(Htmlstring, "select", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "insert", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "delete from", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "count''", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "drop table", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "truncate", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "asc", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "mid", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "char", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "xp_cmdshell", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "exec master", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "net localgroup administrators", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "and", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "net user", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, "or", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, "net", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, "*", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "-", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "delete", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "drop", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "script", "", RegexOptions.IgnoreCase);

                //特殊的字符
                Htmlstring = Htmlstring.Replace("<", "");
                Htmlstring = Htmlstring.Replace(">", "");
                //Htmlstring = Htmlstring.Replace("*", "");
                //Htmlstring = Htmlstring.Replace("-", "");
                Htmlstring = Htmlstring.Replace("?", "");
                Htmlstring = Htmlstring.Replace("'", "''");
                Htmlstring = Htmlstring.Replace(",", "");
                //Htmlstring = Htmlstring.Replace("/", "");
                Htmlstring = Htmlstring.Replace(";", "");
                Htmlstring = Htmlstring.Replace("*/", "");
                Htmlstring = Htmlstring.Replace("\r\n", "");
                Htmlstring = Htmlstring.Trim();

                return Htmlstring;
            }
        }

        /// <summary>
        /// 获取PasswordSalt
        /// </summary>
        /// <param name="isFixed">是否固定字符串</param>
        /// <returns></returns>
        public static string GetPasswordSalt(bool isFixed = false)
        {
            if (isFixed)
            {
                return "y8ce0ydh88";
            }
            else
            {
                return Substring(Guid.NewGuid().ToString("N"), 10, false);
            }
        }

        /// <summary>
        /// 判断是否是邮箱
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format. 
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 判断QQ是否正确
        /// 表达式 ^\d{m,n}$ 例如^\d{7,8}$
        /// 描述 匹配m到n个数字
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidQQ(string strIn)
        {
            return Regex.IsMatch(strIn, @"^\d{4,20}$");
        }


        /// <summary>
        /// 判断是否英文字母或数字的C#正则表达式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNaturalNumber(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return reg1.IsMatch(str);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="Length">长度</param>
        /// <param name="hasEllipsis">是否需要省略号</param>
        /// <returns></returns>
        public static string Substring(string str, int Length, bool hasEllipsis = true)
        {
            if (str.Length > Length)
            {
                if (hasEllipsis)
                {
                    return str.Substring(0, Length) + "...";
                }
                else
                {
                    return str.Substring(0, Length);
                }
            }
            return str;
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">长度</param>
        /// <param name="hasEllipsis">是否需要省略号</param>
        /// <returns></returns>
        public static string SubMiddleString(string str, int length, bool hasEllipsis = true)
        {
            if (str.Length > length)
            {
                int startIndex = (str.Length - length) / 2;

                if (hasEllipsis)
                {
                    return str.Substring(startIndex, length - 2) + "...";
                }
                else
                {
                    return str.Substring(startIndex, length - 2);
                }

            }
            return str;
        }

        /// <summary>
        /// 判断Url是否符合程序要求
        /// </summary>
        /// <returns></returns>
        public static bool UrlTextFilter(string url)
        {
            string[] temp = url.Split('.');
            if (url.EndsWith(".com.cn") ||//判断域名的种类
                url.EndsWith(".gov.cn") ||
                url.EndsWith(".net.cn") ||
                url.EndsWith(".org.cn")
                )
            {
                #region 以.gov.cn|.com.cn|.net.cn|.org.cn结尾的域名处理
                if (temp.Length == 4)//域名分成4段
                {
                    if (url.IndexOf("http://www.") != 0 &&
                        url.IndexOf("http://bbs.") != 0 &&
                        url.IndexOf("http://blog.") != 0 &&
                        url.IndexOf("http://news.") != 0 &&
                        url.IndexOf("http://sports.") != 0
                        )
                    {
                        return false;
                    }
                }
                else
                {
                    if (temp.Length > 4)
                    {
                        return false;
                    }
                }
                #endregion
            }
            else
            {
                #region 以其他域名结尾的处理办法
                if (temp.Length == 3)
                {
                    if (url.IndexOf("http://www.") != 0 &&
                        url.IndexOf("http://bbs.") != 0 &&
                        url.IndexOf("http://blog.") != 0 &&
                        url.IndexOf("http://news.") != 0 &&
                        url.IndexOf("http://sports.") != 0
                        )
                    {
                        return false;
                    }
                }
                else
                {
                    if (temp.Length > 3)
                    {
                        return false;
                    }
                }
                #endregion
            }
            return true;
        }

        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetEncoding(Stream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            fs.Close();
            return reVal;
        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

        /// <summary>
        /// 判断是否包含非法词语
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static bool IsIncludeUnlawful(string strText)
        {
            //是否存在不良信息
            if (strText.Contains("人体艺术") ||
                strText.Contains("人体写真") ||
                strText.Contains("美女写真") ||
                strText.Contains("激情聊天") ||
                strText.Contains("激情图片") ||
                strText.Contains("美女聊天") ||
                strText.Contains("美腿丝袜") ||
                strText.Contains("AV女优") ||
                strText.Contains("一夜情") ||
                strText.Contains("色导航") ||
                strText.Contains("黄色") ||
                strText.Contains("嫩穴") ||
                strText.Contains("乱淫") ||
                strText.Contains("裸体") ||
                strText.Contains("裸聊") ||
                strText.Contains("强奸") ||
                strText.Contains("迷奸") ||
                strText.Contains("阴道") ||
                strText.Contains("做爱") ||
                strText.Contains("性爱") ||
                strText.Contains("A片") ||
                strText.Contains("乱伦") ||
                strText.Contains("三级片") ||
                strText.Contains("情色") ||
                strText.Contains("色情") ||
                strText.Contains("伦理") ||
                strText.Contains("美女") ||
                strText.Contains("性交") ||
                strText.Contains("少妇") ||
                strText.Contains("美女图片"))
            {
                return true;
            }

            return false;
        }
    }
}
