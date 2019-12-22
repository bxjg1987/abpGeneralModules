using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    /// <summary>
    /// 获取树形下拉框数据的模型
    /// </summary>
    public class GeneralTreeNodeDto
    {
        public string id { get; set; }
        public string text { get; set; }
        public string iconCls { get; set; }
        public bool @checked { get; set; }
        public string state { get; set; } = "open";
        public dynamic attributes { get; set; }
        public IList<GeneralTreeNodeDto> children { get; set; }

        public string parentId { get; set; }

        ////因为不确定前端是否支持这样自定义的字段，因此保险起见数据都保存到attributes，这里只提供读取
        public string code
        {
            get
            {
                if (attributes == null)
                    return null;

                var dic = this.attributes as IDictionary<string, object>;
                if (dic == null)
                    return null;

                if (dic.ContainsKey("code"))
                    return attributes.code;

                return null;
            }
        }
        public dynamic extData
        {
            get
            {
                if (attributes == null)
                    return null;

                var dic = this.attributes as IDictionary<string, object>;
                if (dic == null)
                    return null;

                if (dic.ContainsKey("extData"))
                    return attributes.extData;

                return null;


                //return attributes!=null? attributes.extData:null;

            }
        }
    }
}
