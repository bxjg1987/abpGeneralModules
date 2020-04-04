using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZLJ.Utils.Extensions
{
    public static class HashHelper
    {
        public static string GetMD5(this Stream stream)
        {
            byte[] retVal = null;

            using (var md5 = MD5.Create())
            {
                retVal = md5.ComputeHash(stream);
            }

            if (retVal == null || retVal.Length == 0)
                throw new Exception();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string GetMD5ByFilePath(this string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                return file.GetMD5();
            }
        }
       /// <summary>
       /// 计算指定字符串的32位md5值
       /// 微信小程序支付签名算法使用的此方式，编码为utf8
       /// 已转换为大写
       /// </summary>
       /// <param name="str"></param>
       /// <param name="encoding"></param>
       /// <returns></returns>
        public static string GetMD532(this string str, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var bs = encoding.GetBytes(str);
            byte[] jmh;
            using (var md5 = MD5.Create())
            {
                jmh = md5.ComputeHash(bs);
            }
            StringBuilder sb1 = new StringBuilder();
            for (int i = 0; i < jmh.Length; i++)
            {
                sb1.Append(jmh[i].ToString("X2"));
            }
            return sb1.ToString();
        }
    }
}
