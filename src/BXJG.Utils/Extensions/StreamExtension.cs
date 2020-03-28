using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZLJ.Utils
{
    public static class StreamExtension
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
    }
}
