using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.Utils.Enums
{
    /// <summary>
    /// 枚举作为下拉框时
    /// </summary>
    public class EnumConfigItem
    {
        public EnumConfigItem()
        {
        }

        public EnumConfigItem(string name, Type type, string locationSourceName)
        {
            Name = name;
            Type = type;
            LocationSourceName = locationSourceName;
        }

        public string Name { get; set; }
        public Type Type { get; set; }
        public string LocationSourceName { get; set; }
    }
}
