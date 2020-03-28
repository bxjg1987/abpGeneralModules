using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace BXJG.Utils.XML
{
    public static class XMLHelper
    {
        //ValueTask参考5min+ https://www.lizenghai.com/archives/44685.html
        public async static Task<T> XmlDeserializeAsync<T>(this Stream stream, Encoding encoding = null)
            where T : class
        {
            //Deserialize目前没有异步方法，所以用了奇葩方式

            //using是新语法，猜测是在变量未被引用地方立即释放，微软大法好...

            using var st = new StreamReader(stream,encoding);
            var str = await st.ReadToEndAsync();
            using var sr = new StringReader(str);
            var sfd = new XmlSerializer(typeof(T));
            return sfd.Deserialize(sr) as T;
        }
    }
}
