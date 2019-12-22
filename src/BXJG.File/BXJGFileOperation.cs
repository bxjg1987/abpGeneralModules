using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.File
{
    /// <summary>
    /// 文件操作类型
    /// </summary>
    public enum BXJGFileOperation
    {
        /// <summary>
        /// 上传
        /// </summary>
        Upload,
        /// <summary>
        /// 下载
        /// </summary>
        Download,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 移动
        /// </summary>
        Move,
        /// <summary>
        /// 修改
        /// </summary>
        Modify
    }
}
