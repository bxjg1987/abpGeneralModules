using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
namespace BXJG.Utils.XML
{
    public static class XMLHelper
    {
        //ValueTask参考5min+ https://www.lizenghai.com/archives/44685.html
        public static Task<T> XmlDeserializeAsync<T>(this Stream stream, Encoding encoding = null, CancellationToken cancellation = default)
            where T : class
        {
            //Deserialize目前没有异步方法，所以用了奇葩方式 不确定是不是错误的方式

            //using是新语法，猜测是在变量未被引用地方立即释放，微软大法好...
            //encoding = encoding ?? Encoding.UTF8;
            //using var st = new StreamReader(stream,encoding);
            //var str = await st.ReadToEndAsync();
            //using var sr = new StringReader(str);
            //var sfd = new XmlSerializer(typeof(T));
            //return sfd.Deserialize(sr) as T;
       
            encoding = encoding ?? Encoding.UTF8;
            using var st = new StreamReader(stream, encoding);
            var sfd = new XmlSerializer(typeof(T));
            return Task.FromResult(sfd.Deserialize(st) as T);
        }

        public static Task XmlSerialize<T>(this Stream stream, T t, Encoding encoding = null, CancellationToken cancellation = default)
        {
            //还是因为没有提供异步方法，可以考虑序列号为byte[] 然后stream.WriteAsync
            encoding = encoding ?? Encoding.UTF8;
            using var st = new StreamWriter(stream, encoding);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));//实验发现这里用object类型，序列化时会报错
            xmlSerializer.Serialize(st, t);
            return Task.CompletedTask;
        }
    }
}
