using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BXJG.Utils
{
    /// <summary>
    /// 生成随机数
    /// </summary>
    public static class RandomHelper
    {
        static RandomNumberGenerator rng = RandomNumberGenerator.Create();
        /// <summary>
        /// a-z A-Z 0-9 生成的随机值 性能略高于Random255
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomBase64(int length = 6)
        {
            var bs = new byte[length];
            rng.GetBytes(bs);
            var str = Convert.ToBase64String(bs)
                .Replace("/", "0")
                .Replace("+", "1")
                .Replace("=", "2")
                .Substring(0, length);
            return str;
        }
        /// <summary>
        /// 从指定字符串中生成随机值，做想生成a-z A-Z 0-9的随机值 RandomBase64方法性能略高
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Random255(string str, byte length = 6)
        {
            var bs = new byte[length];
            rng.GetBytes(bs);
            char[] ary = new char[length];
            for (int i = 0; i < length; i++)
            {
                var j = Convert.ToInt32(bs[i]);
                var k = j % str.Length;
                ary[i] = str[k];
            }
            return new string(ary);
        }
        /// <summary>
        /// 随机纯数字的字符串
        /// 比如生成手机验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomNumber(byte length = 6)
        {
            //也许有更好的实现方式
            return Random255("0123456789", length);
        }
    }
}
