using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Utils.Dto
{
    /*
     * 主要针对获取下拉框需要的数据时 所提供的输入参数
     * 
     * 比如：数据字典、角色、枚举....
     * 
     * 可能需要提供一个默认值来表示未选择任何项。比如在用户列表页的角色下拉框，用来显示指定角色的用户，此时下拉框不提供label，默认直接在下拉框中加入一个“==角色==”
     * 同理在表单页面可能是    角色：==请选择==
     * 这个数据的来源分3种情况
     *  1、前端指定
     *  2、类似数据字典的情况，可能直接以父节点名字作为这个节点名
     *  3、用默认的， 比如：==区域==
     * 
     * 以上情况通过ForType，请参考它的说明，
     * 为了方便将来扩展，直接使用int类型
     * 
     */

    /// <summary>
    /// 列表页搜索框、表单页下拉框查询使用的输入模型
    /// </summary>
    public class GetForSelectInput
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
        public int ForType { get; set; }
        /// <summary>
        /// 给前端一个机会来设置自己想要的值，注意是否需要本地化调用方来决定
        /// </summary>
        public string ParentText { get; set; }
    }
}
