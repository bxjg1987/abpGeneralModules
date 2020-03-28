using BXJG.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.Utils
{
    public class BXJGUtilsModuleConfig
    {
        //最终处理办法是属性类型直接用List 而不是IList
        public List<EnumConfigItem> Enums { get; set; }=  new List<EnumConfigItem>();

        public BXJGUtilsModuleConfig()
        {
            //诡异的问题，在这里初始化后，  调用方从容器中获取或注入的单例对象的Enums是一个数组
            //Enums = new List<EnumConfigItem>();

        }
    }
}
