using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    /// <summary>
    /// 列表页搜索框、表单页下拉框查询使用的输入模型
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class GeneralTreeGetForSelectInput : BXJG.Utils.Dto.GetForSelectInput
    {
        /// <summary>
        /// 0直接返回查找到的后代节点，
        /// 
        /// 大于0的都会尝试增加父节点的返回
        /// 若ParentText不为空，则取ParentText的本地化文本后创建一个虚拟节点作为父节点，子节点为查询到的列表
        /// 1若ParentText为空，则尝试加载Id对应的父节点，若未找到则通过Search创建虚拟节点
        /// 2若未找到则通过Search创建虚拟节点
        ///   
        /// 3若ParentText为空，则尝试加载Id对应的父节点，若未找到则通过Form创建虚拟节点
        /// 4若未找到则通过Form创建虚拟节点
        /// </summary>
        //public int ForType { get; set; }
        /// <summary>
        /// 给前端一个机会来设置自己想要的值，注意是否需要本地化调用方来决定
        /// </summary>
       // public string ParentText { get; set; }

       public long? ParentId { get; set; }
    }
}
