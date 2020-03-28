using BXJG.Utils.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Utils.Application
{
    public class GetEnumForCombboxInput : GetForCombboxInput
    {
        /// <summary>
        /// 是否加载“未知”
        /// </summary>
        public bool Nullable { get; set; }
        /// <summary>
        /// 枚举类型名
        /// </summary>
        [Required]
        public string EnumTypeName { get; set; }
    }
}
