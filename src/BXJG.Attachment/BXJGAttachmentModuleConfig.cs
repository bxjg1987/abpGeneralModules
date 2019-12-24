using BXJG.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.Attachment
{
    /// <summary>
    /// 附件模块配置对象
    /// </summary>
    public class BXJGAttachmentModuleConfig
    {
        /// <summary>
        /// 获取或设置附件权限定义
        /// </summary>
        public ICollection<BXJGAttachmentPermission> Permissions = new List<BXJGAttachmentPermission>();//配置对象是单例的，所以可以不加static
        /// <summary>
        /// 添加一个附件权限定义
        /// 你可以直接访问Permissions属性注册权限，但使用此方法更简单
        /// </summary>
        /// <param name="module"></param>
        /// <param name="operation"></param>
        /// <param name="permissions"></param>
        public void AddPermission(string module, BXJGFileOperation operation, params string[] permissions)
        {
            Permissions.Add(new BXJGAttachmentPermission(module, operation, permissions));
        }
    }
}
