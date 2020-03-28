using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLJ.Utils.Extensions
{
   public static  class StringExtension
    {
        public static string GetMD5ByFilePath(this string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                return file.GetMD5();
            }
        }
    }
}
