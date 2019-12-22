using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    /// <summary>
    /// 移动树形节点时的请求参数
    /// </summary>
    public class GeneralTreeNodeMoveInput
    {
        /// <summary>
        /// MoveTargetRelativePosition若为Append则此字段表示父节点id，否则表示目标节点id
        /// </summary>
        public long? TargetId { get; set; }
        /// <summary>
        /// 源节点id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 移动到目标节点的相对位置
        /// </summary>
        public GeneralTreeMoveType MoveType { get; set; }

    }
    ///// <summary>
    ///// 移动到目标节点的相对位置
    ///// </summary>
    //public enum TreeNodeMoveTargetRelativePosition
    //{
    //    /// <summary>
    //    /// 追加
    //    /// </summary>
    //    Append,
    //    /// <summary>
    //    /// 之前
    //    /// </summary>
    //    Front,
    //    /// <summary>
    //    /// 之后
    //    /// </summary>
    //    After
    //}
}
