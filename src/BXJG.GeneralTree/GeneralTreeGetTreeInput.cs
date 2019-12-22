using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    /// <summary>
    /// 树形数据的管理页面查询时的请求模型
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class GeneralTreeGetTreeInput<TId> // 与ForSelectInput不同，后者只针对选择，且有Search和Form模式
    {
        //不要使用Nullable<TId>类型，因为某些情况此参数可能不允许为null
        //若数据很多 可以需要么次加载一个节点的数据，所以ParentId字段是必要的

        /// <summary>
        /// 父级节点id，非必填
        /// </summary>
        public TId ParentId { get; set; } 
        /// <summary>
        /// 是否加载父节点
        /// </summary>
        public bool LoadParent { get; set; }
        /// <summary>
        /// 若LoadParent为true且ParentId为空，则自动以{id:0,text:L( ParentText) }创建一个虚拟父节点，注意L是处理本地化的方法，因此ParentText值的是本地化的key
        /// </summary>
        public string ParentText { get; set; }

        //没必要在这里定义初始值，因为需要处理本地化
        //若想在这里处理需要引入LocationManager，但是这个类是被模型绑定实例化的，因此比较麻烦，不确定abp的模型绑定是否注入本地化相关类，所以不再这个类的内部来做本地化
    }
}
