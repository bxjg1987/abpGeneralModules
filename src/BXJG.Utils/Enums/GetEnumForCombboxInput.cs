using BXJG.Utils.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Utils.Enums
{
    /// <summary>
    /// 获取枚举作为下拉框数据时的输入模型
    /// </summary>
    public class GetEnumForCombboxInput : GetForSelectInput
    {
        ///// <summary>
        ///// 是否加载“未知”
        ///// </summary>
        //public bool Nullable { get; set; }
        /// <summary>
        /// 枚举类型名
        /// </summary>
        [Required]
        public string EnumTypeName { get; set; }
    }
}
