using Abp.Application.Services.Dto;
using Abp.AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    /// <summary>
    /// 树形数据管理的列表页使用的dto基类，它不是抽象的，可以直接使用
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    public class GeneralTreeGetTreeNodeBaseDto<TChild> : AuditedEntityDto<long>
        where TChild: GeneralTreeGetTreeNodeBaseDto<TChild>
    {
        /// <summary>
        /// 父级组织单位id
        /// </summary>
        public long? ParentId { get; set; }
        /// <summary>
        /// 有层次结构的代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        public IList<TChild> Children { get; set; }
        /// <summary>
        /// 配合easyui，state：节点状态，'open' 或 'closed'，默认：'open'。如果为'closed'的时候，将不自动展开该节点。
        /// </summary>
        public string State { get; set; } = "open";
        //{
        //    get
        //    {
        //        return Children!=null&& Children.Count > 0&& Id!=0 ? "closed" : "open";
        //    }
        //}
        /// <summary>
        /// 什么鬼
        /// </summary>
        public string Text { get { return DisplayName; } }
        ///// <summary>
        ///// 排序索引
        ///// </summary>
        //public int OrderIndex { get; set; }

        public dynamic ExtData { get; set; }
    }
}
