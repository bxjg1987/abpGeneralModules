using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BXJG.File;
using Abp;

namespace BXJG.Attachment
{
    /// <summary>
    /// 附件权限检查接口
    /// </summary>
    public interface IBXJGAttachmentPermissionChecker : ITransientDependency
    {
        /// <summary>
        /// 验证当前用户 对指定模块的 指定附件操作 的指定权限
        /// </summary>
        /// <param name="module">模块名</param>
        /// <param name="act">附件动作</param>
        /// <param name="permission">权限名</param>
        /// <returns></returns>
        Task<BXJGAttachmentCheckPermissionResult> CheckPermissionAsync(string module, BXJGFileOperation act, string permission);
        /// <summary>
        /// 验证指定用户 对指定模块的 指定附件操作 的指定权限
        /// </summary>
        /// <param name="module">模块名</param>
        /// <param name="act">附件动作</param>
        /// <param name="permission">权限名</param>
        /// <param name="userIdentifier">用户标识</param>
        /// <returns></returns>
        Task<BXJGAttachmentCheckPermissionResult> CheckPermissionAsync(UserIdentifier userIdentifier, string module, BXJGFileOperation act, string permission);
    }
}
