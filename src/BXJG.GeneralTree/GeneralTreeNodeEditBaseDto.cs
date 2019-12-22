/*
 * 作者：变形精怪 手机/微信17723896676 QQ/邮箱453008453
 * 创建时间：2018-10-10 23:53:10
 *
 * 说明：略...
 */
using Abp.Application.Services.Dto;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    /// <summary>
    /// 树形结构抽象编辑模型
    /// </summary>
    public class GeneralTreeNodeEditBaseDto : IEntityDto<long>
    {
        /// <summary>
        /// Id
        /// </summary>
        //[Range(0, long.MaxValue)]
        public long Id { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        public long? ParentId { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        [Required]
        [StringLength(GeneralTreeEntity.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
        ///// <summary>
        ///// 排序索引
        ///// </summary>
        //public int OrderIndex { get; set; }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public IDictionary<string, object> ExtData { get; set; } //访问时由于可以配合nameof，对dynamic访问更简单

        //public dynamic ExtData { get; set; }
    }
}
