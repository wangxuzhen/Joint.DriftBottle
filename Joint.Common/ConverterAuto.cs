// ***********************************************************************
// Project          : Micua Infrastructure
// Assembly         : Micua.Infrastructure.Utility
// Author           : iceStone
// Created          : 2014-01-05 11:27
// 
// Last Modified By : iceStone
// Last Modified On : 2014-06-23 16:56
// ***********************************************************************
// <copyright file="Convert.cs" company="Wedn.Net">
//     Copyright © 2014 Wedn.Net. All Rights Reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace System
{
    /// <summary>
    /// 转换拓展方法
    /// </summary>
    public static partial class Converter
    {
        #region Convert string type to other types


        #region String to Boolean +static bool ToBoolean(this string s, bool def = default(Boolean))
        /// <summary>
        /// String to Boolean(字符串 转换成 布尔型)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static bool ToBoolean(this string s, bool def = default(bool))
        {
            bool result;
            return Boolean.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to Char +static char ToChar(this string s, char def = default(Char))
        /// <summary>
        /// String to Char(字符串 转换成 无符号、数值、整数)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static char ToChar(this string s, char def = default(char))
        {
            char result;
            return Char.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to Decimal +static decimal ToDecimal(this string s, decimal def = default(Decimal))
        /// <summary>
        /// String to Decimal(字符串 转换成 数值、十进制)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static decimal ToDecimal(this string s, decimal def = default(decimal))
        {
            decimal result;
            return Decimal.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to Double +static double ToDouble(this string s, double def = default(Double))
        /// <summary>
        /// String to Double(字符串 转换成 数值、浮点)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static double ToDouble(this string s, double def = default(double))
        {
            double result;
            return Double.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to Single +static float ToSingle(this string s, float def = default(Single))
        /// <summary>
        /// String to Single(字符串 转换成 数值、浮点)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static float ToSingle(this string s, float def = default(float))
        {
            float result;
            return Single.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to Byte +static byte ToByte(this string s, byte def = default(Byte))
        /// <summary>
        /// String to Byte(字符串 转换成 无符号、数值、整数)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static byte ToByte(this string s, byte def = default(byte))
        {
            byte result;
            return Byte.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to SByte +static sbyte ToSByte(this string s, sbyte def = default(SByte))
        /// <summary>
        /// String to SByte(字符串 转换成 有符号、数值、整数)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static sbyte ToSByte(this string s, sbyte def = default(sbyte))
        {
            sbyte result;
            return SByte.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to Int16 +static short ToInt16(this string s, short def = default(Int16))
        /// <summary>
        /// String to Int16(字符串 转换成 有符号、数值、整数)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static short ToInt16(this string s, short def = default(short))
        {
            short result;
            return Int16.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to UInt16 +static ushort ToUInt16(this string s, ushort def = default(UInt16))
        /// <summary>
        /// String to UInt16(字符串 转换成 无符号、数值、整数)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static ushort ToUInt16(this string s, ushort def = default(ushort))
        {
            ushort result;
            return UInt16.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to Int32 +static int ToInt32(this string s, int def = default(Int32))
        /// <summary>
        /// String to Int32(字符串 转换成 有符号、数值、整数)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static int ToInt32(this string s, int def = default(int))
        {
            int result;
            return Int32.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to UInt32 +static uint ToUInt32(this string s, uint def = default(UInt32))
        /// <summary>
        /// String to UInt32(字符串 转换成 无符号、数值、整数)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static uint ToUInt32(this string s, uint def = default(uint))
        {
            uint result;
            return UInt32.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to Int64 +static long ToInt64(this string s, long def = default(Int64))
        /// <summary>
        /// String to Int64(字符串 转换成 有符号、数值、整数)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static long ToInt64(this string s, long def = default(long))
        {
            long result;
            return Int64.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to UInt64 +static ulong ToUInt64(this string s, ulong def = default(UInt64))
        /// <summary>
        /// String to UInt64(字符串 转换成 无符号、数值、整数)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static ulong ToUInt64(this string s, ulong def = default(ulong))
        {
            ulong result;
            return UInt64.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to DateTime +static DateTime ToDateTime(this string s, DateTime def = default(DateTime))
        /// <summary>
        /// String to DateTime(字符串 转换成 时间)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static DateTime ToDateTime(this string s, DateTime def = default(DateTime))
        {
            DateTime result;
            return DateTime.TryParse(s, out result) ? result : def;
        }
        #endregion


        #region String to Guid +static Guid ToGuid(this string s, Guid def = default(Guid))
        /// <summary>
        /// String to Guid(字符串 转换成 Guid)
        /// </summary>
        /// <remarks>
        ///  2014-06-23 16:31 Created By iceStone
        /// </remarks>
        /// <param name="s">String</param>
        /// <param name="def">Default</param>
        /// <returns>Byte</returns>
        public static Guid ToGuid(this string s, Guid def = default(Guid))
        {
            Guid result;
            return Guid.TryParse(s, out result) ? result : def;
        }
        #endregion


        /// <summary>获取中文简拼</summary>     
        /// <param name="ChineseStr">中文字符串</param>     
        /// <returns>首字母</returns>      
        public static string GetChineseHeadSpell(this string ChineseStr)
        {
            if (ChineseStr == string.Empty)
            {
                return string.Empty;
            }

            string Capstr = string.Empty;
            byte[] ZW = new byte[2];
            long ChineseStr_int;
            string CharStr, ChinaStr = "";
            for (int i = 0; i <= ChineseStr.Length - 1; i++)
            {
                CharStr = ChineseStr.Substring(i, 1).ToString();
                ZW = System.Text.Encoding.Default.GetBytes(CharStr); // 得到汉字符的字节数组      

                if (ZW.Length == 2)
                {
                    int i1 = (short)(ZW[0]);
                    int i2 = (short)(ZW[1]);
                    ChineseStr_int = i1 * 256 + i2;
                    if ((ChineseStr_int >= 45217) && (ChineseStr_int <= 45252)) { ChinaStr = "a"; }
                    else if ((ChineseStr_int >= 45253) && (ChineseStr_int <= 45760)) { ChinaStr = "b"; }
                    else if ((ChineseStr_int >= 45761) && (ChineseStr_int <= 46317)) { ChinaStr = "c"; }
                    else if ((ChineseStr_int >= 46318) && (ChineseStr_int <= 46825)) { ChinaStr = "d"; }
                    else if ((ChineseStr_int >= 46826) && (ChineseStr_int <= 47009)) { ChinaStr = "e"; }
                    else if ((ChineseStr_int >= 47010) && (ChineseStr_int <= 47296)) { ChinaStr = "f"; }
                    else if ((ChineseStr_int >= 47297) && (ChineseStr_int <= 47613)) { ChinaStr = "g"; }
                    else if ((ChineseStr_int >= 47614) && (ChineseStr_int <= 48118)) { ChinaStr = "h"; }
                    else if ((ChineseStr_int >= 48119) && (ChineseStr_int <= 49061)) { ChinaStr = "j"; }
                    else if ((ChineseStr_int >= 49062) && (ChineseStr_int <= 49323)) { ChinaStr = "k"; }
                    else if ((ChineseStr_int >= 49324) && (ChineseStr_int <= 49895)) { ChinaStr = "l"; }
                    else if ((ChineseStr_int >= 49896) && (ChineseStr_int <= 50370)) { ChinaStr = "m"; }
                    else if ((ChineseStr_int >= 50371) && (ChineseStr_int <= 50613)) { ChinaStr = "n"; }
                    else if ((ChineseStr_int >= 50614) && (ChineseStr_int <= 50621)) { ChinaStr = "o"; }
                    else if ((ChineseStr_int >= 50622) && (ChineseStr_int <= 50905)) { ChinaStr = "p"; }
                    else if ((ChineseStr_int >= 50906) && (ChineseStr_int <= 51386)) { ChinaStr = "q"; }
                    else if ((ChineseStr_int >= 51387) && (ChineseStr_int <= 51445)) { ChinaStr = "r"; }
                    else if ((ChineseStr_int >= 51446) && (ChineseStr_int <= 52217)) { ChinaStr = "s"; }
                    else if ((ChineseStr_int >= 52218) && (ChineseStr_int <= 52697)) { ChinaStr = "t"; }
                    else if ((ChineseStr_int >= 52698) && (ChineseStr_int <= 52979)) { ChinaStr = "w"; }
                    else if ((ChineseStr_int >= 52980) && (ChineseStr_int <= 53640)) { ChinaStr = "x"; }
                    else if ((ChineseStr_int >= 53689) && (ChineseStr_int <= 54480)) { ChinaStr = "y"; }
                    else if ((ChineseStr_int >= 54481) && (ChineseStr_int <= 55289)) { ChinaStr = "z"; }
                }
                else
                    Capstr += CharStr;
                Capstr += ChinaStr;
            }
            return Capstr;
        }

        /// <summary>
        /// 获取中文拼音
        /// </summary>
        /// <param name="x">中文字符串</param>
        /// <returns>拼音</returns>
        public static string GetSpell(this string x)
        {
            int[] iA = new int[]  
              {  
                   -20319 ,-20317 ,-20304 ,-20295 ,-20292 ,-20283 ,-20265 ,-20257 ,-20242 ,-20230  
                   ,-20051 ,-20036 ,-20032 ,-20026 ,-20002 ,-19990 ,-19986 ,-19982 ,-19976 ,-19805  
                   ,-19784 ,-19775 ,-19774 ,-19763 ,-19756 ,-19751 ,-19746 ,-19741 ,-19739 ,-19728  
                   ,-19725 ,-19715 ,-19540 ,-19531 ,-19525 ,-19515 ,-19500 ,-19484 ,-19479 ,-19467  
                   ,-19289 ,-19288 ,-19281 ,-19275 ,-19270 ,-19263 ,-19261 ,-19249 ,-19243 ,-19242  
                   ,-19238 ,-19235 ,-19227 ,-19224 ,-19218 ,-19212 ,-19038 ,-19023 ,-19018 ,-19006  
                   ,-19003 ,-18996 ,-18977 ,-18961 ,-18952 ,-18783 ,-18774 ,-18773 ,-18763 ,-18756  
                   ,-18741 ,-18735 ,-18731 ,-18722 ,-18710 ,-18697 ,-18696 ,-18526 ,-18518 ,-18501  
                   ,-18490 ,-18478 ,-18463 ,-18448 ,-18447 ,-18446 ,-18239 ,-18237 ,-18231 ,-18220  
                   ,-18211 ,-18201 ,-18184 ,-18183 ,-18181 ,-18012 ,-17997 ,-17988 ,-17970 ,-17964  
                   ,-17961 ,-17950 ,-17947 ,-17931 ,-17928 ,-17922 ,-17759 ,-17752 ,-17733 ,-17730  
                   ,-17721 ,-17703 ,-17701 ,-17697 ,-17692 ,-17683 ,-17676 ,-17496 ,-17487 ,-17482  
                   ,-17468 ,-17454 ,-17433 ,-17427 ,-17417 ,-17202 ,-17185 ,-16983 ,-16970 ,-16942  
                   ,-16915 ,-16733 ,-16708 ,-16706 ,-16689 ,-16664 ,-16657 ,-16647 ,-16474 ,-16470  
                   ,-16465 ,-16459 ,-16452 ,-16448 ,-16433 ,-16429 ,-16427 ,-16423 ,-16419 ,-16412  
                   ,-16407 ,-16403 ,-16401 ,-16393 ,-16220 ,-16216 ,-16212 ,-16205 ,-16202 ,-16187  
                   ,-16180 ,-16171 ,-16169 ,-16158 ,-16155 ,-15959 ,-15958 ,-15944 ,-15933 ,-15920  
                   ,-15915 ,-15903 ,-15889 ,-15878 ,-15707 ,-15701 ,-15681 ,-15667 ,-15661 ,-15659  
                   ,-15652 ,-15640 ,-15631 ,-15625 ,-15454 ,-15448 ,-15436 ,-15435 ,-15419 ,-15416  
                   ,-15408 ,-15394 ,-15385 ,-15377 ,-15375 ,-15369 ,-15363 ,-15362 ,-15183 ,-15180  
                   ,-15165 ,-15158 ,-15153 ,-15150 ,-15149 ,-15144 ,-15143 ,-15141 ,-15140 ,-15139  
                   ,-15128 ,-15121 ,-15119 ,-15117 ,-15110 ,-15109 ,-14941 ,-14937 ,-14933 ,-14930  
                   ,-14929 ,-14928 ,-14926 ,-14922 ,-14921 ,-14914 ,-14908 ,-14902 ,-14894 ,-14889  
                   ,-14882 ,-14873 ,-14871 ,-14857 ,-14678 ,-14674 ,-14670 ,-14668 ,-14663 ,-14654  
                   ,-14645 ,-14630 ,-14594 ,-14429 ,-14407 ,-14399 ,-14384 ,-14379 ,-14368 ,-14355  
                   ,-14353 ,-14345 ,-14170 ,-14159 ,-14151 ,-14149 ,-14145 ,-14140 ,-14137 ,-14135  
                   ,-14125 ,-14123 ,-14122 ,-14112 ,-14109 ,-14099 ,-14097 ,-14094 ,-14092 ,-14090  
                   ,-14087 ,-14083 ,-13917 ,-13914 ,-13910 ,-13907 ,-13906 ,-13905 ,-13896 ,-13894  
                   ,-13878 ,-13870 ,-13859 ,-13847 ,-13831 ,-13658 ,-13611 ,-13601 ,-13406 ,-13404  
                   ,-13400 ,-13398 ,-13395 ,-13391 ,-13387 ,-13383 ,-13367 ,-13359 ,-13356 ,-13343  
                   ,-13340 ,-13329 ,-13326 ,-13318 ,-13147 ,-13138 ,-13120 ,-13107 ,-13096 ,-13095  
                   ,-13091 ,-13076 ,-13068 ,-13063 ,-13060 ,-12888 ,-12875 ,-12871 ,-12860 ,-12858  
                   ,-12852 ,-12849 ,-12838 ,-12831 ,-12829 ,-12812 ,-12802 ,-12607 ,-12597 ,-12594  
                   ,-12585 ,-12556 ,-12359 ,-12346 ,-12320 ,-12300 ,-12120 ,-12099 ,-12089 ,-12074  
                   ,-12067 ,-12058 ,-12039 ,-11867 ,-11861 ,-11847 ,-11831 ,-11798 ,-11781 ,-11604  
                   ,-11589 ,-11536 ,-11358 ,-11340 ,-11339 ,-11324 ,-11303 ,-11097 ,-11077 ,-11067  
                   ,-11055 ,-11052 ,-11045 ,-11041 ,-11038 ,-11024 ,-11020 ,-11019 ,-11018 ,-11014  
                   ,-10838 ,-10832 ,-10815 ,-10800 ,-10790 ,-10780 ,-10764 ,-10587 ,-10544 ,-10533  
                   ,-10519 ,-10331 ,-10329 ,-10328 ,-10322 ,-10315 ,-10309 ,-10307 ,-10296 ,-10281  
                   ,-10274 ,-10270 ,-10262 ,-10260 ,-10256 ,-10254  
                  };
            string[] sA = new string[]  
          {  
           "a","ai","an","ang","ao"  
  
           ,"ba","bai","ban","bang","bao","bei","ben","beng","bi","bian","biao","bie","bin"  
           ,"bing","bo","bu"  
  
           ,"ca","cai","can","cang","cao","ce","ceng","cha","chai","chan","chang","chao","che"  
           ,"chen","cheng","chi","chong","chou","chu","chuai","chuan","chuang","chui","chun"  
           ,"chuo","ci","cong","cou","cu","cuan","cui","cun","cuo"  
  
           ,"da","dai","dan","dang","dao","de","deng","di","dian","diao","die","ding","diu"  
           ,"dong","dou","du","duan","dui","dun","duo"  
  
           ,"e","en","er"  
  
           ,"fa","fan","fang","fei","fen","feng","fo","fou","fu"  
  
           ,"ga","gai","gan","gang","gao","ge","gei","gen","geng","gong","gou","gu","gua","guai"  
           ,"guan","guang","gui","gun","guo"  
  
           ,"ha","hai","han","hang","hao","he","hei","hen","heng","hong","hou","hu","hua","huai"  
           ,"huan","huang","hui","hun","huo"  
  
           ,"ji","jia","jian","jiang","jiao","jie","jin","jing","jiong","jiu","ju","juan","jue"  
           ,"jun"  
  
           ,"ka","kai","kan","kang","kao","ke","ken","keng","kong","kou","ku","kua","kuai","kuan"  
           ,"kuang","kui","kun","kuo"  
  
           ,"la","lai","lan","lang","lao","le","lei","leng","li","lia","lian","liang","liao","lie"  
           ,"lin","ling","liu","long","lou","lu","lv","luan","lue","lun","luo"  
  
           ,"ma","mai","man","mang","mao","me","mei","men","meng","mi","mian","miao","mie","min"  
           ,"ming","miu","mo","mou","mu"  
  
           ,"na","nai","nan","nang","nao","ne","nei","nen","neng","ni","nian","niang","niao","nie"  
           ,"nin","ning","niu","nong","nu","nv","nuan","nue","nuo"  
  
           ,"o","ou"  
  
           ,"pa","pai","pan","pang","pao","pei","pen","peng","pi","pian","piao","pie","pin","ping"  
           ,"po","pu"  
  
           ,"qi","qia","qian","qiang","qiao","qie","qin","qing","qiong","qiu","qu","quan","que"  
           ,"qun"  
  
           ,"ran","rang","rao","re","ren","reng","ri","rong","rou","ru","ruan","rui","run","ruo"  
  
           ,"sa","sai","san","sang","sao","se","sen","seng","sha","shai","shan","shang","shao","she"  
           ,"shen","sheng","shi","shou","shu","shua","shuai","shuan","shuang","shui","shun","shuo","si"  
           ,"song","sou","su","suan","sui","sun","suo"  
  
           ,"ta","tai","tan","tang","tao","te","teng","ti","tian","tiao","tie","ting","tong","tou","tu"  
           ,"tuan","tui","tun","tuo"  
  
           ,"wa","wai","wan","wang","wei","wen","weng","wo","wu"  
  
           ,"xi","xia","xian","xiang","xiao","xie","xin","xing","xiong","xiu","xu","xuan","xue","xun"  
  
           ,"ya","yan","yang","yao","ye","yi","yin","ying","yo","yong","you","yu","yuan","yue","yun"  
  
           ,"za","zai","zan","zang","zao","ze","zei","zen","zeng","zha","zhai","zhan","zhang","zhao"  
           ,"zhe","zhen","zheng","zhi","zhong","zhou","zhu","zhua","zhuai","zhuan","zhuang","zhui"  
           ,"zhun","zhuo","zi","zong","zou","zu","zuan","zui","zun","zuo"  
          };
            byte[] B = new byte[2];
            string s = "";
            char[] c = x.ToCharArray();
            for (int j = 0; j < c.Length; j++)
            {
                B = System.Text.Encoding.Default.GetBytes(c[j].ToString());
                if ((int)(B[0]) <= 160 && (int)(B[0]) >= 0)
                {
                    s += c[j];
                }
                else
                {
                    for (int i = (iA.Length - 1); i >= 0; i--)
                    {
                        if (iA[i] <= (int)(B[0]) * 256 + (int)(B[1]) - 65536)
                        {
                            s += sA[i];
                            break;
                        }
                    }
                }
            }
            return s;
        }


        #endregion
    }
}
