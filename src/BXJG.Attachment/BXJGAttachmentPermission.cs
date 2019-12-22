using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BXJG.File;

namespace BXJG.Attachment
{
    //整个思路参考abp源码的 权限定义、上下文、和权限提供器
    //或者参考菜单

    /// <summary>
    /// 附件权限定义
    /// </summary>
    public class BXJGAttachmentPermission
    {
        /// <summary>
        /// 实例化一个附件权限定义
        /// </summary>
        /// <param name="module">模块，如：设备档案</param>
        /// <param name="operation">操作，如：FileOperation.Upload</param>
        /// <param name="permissions">关联的权限集合，如：Admin...Equipment.Create, Admin...Equipment.Update</param>
        public BXJGAttachmentPermission(string module, BXJGFileOperation operation, params string[] permissions)
        {
            this.Module = module;
            this.Operation = operation;
            this.Permissions = permissions;
        }
        /// <summary>
        /// 模块，如：EquipmentInfo
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// 操作，如：FileOperation.Upload
        /// </summary>
        public BXJGFileOperation Operation { get; set; }
        /// <summary>
        /// 关联的权限集合，如：Admin...Equipment.Create, Admin...Equipment.Update
        /// </summary>
        public string[] Permissions { get; set; }
    }
}
