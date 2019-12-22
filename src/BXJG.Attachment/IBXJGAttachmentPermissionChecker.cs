using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BXJG.File;

namespace BXJG.Attachment
{
    /// <summary>
    /// 附件权限检查接口
    /// </summary>
    public interface IBXJGAttachmentPermissionChecker: ITransientDependency
    {
        //由于附件权限是建立在abp权限基础上的，开发时可以定死的，所以不需要定义权限配置之类的
        //也可以考虑对FileOperation对应定义一个方法

        /// <summary>
        /// 验证附件操作权限
        /// </summary>
        /// <param name="module">模块名</param>
        /// <param name="act">附件动作</param>
        /// <param name="permission">权限名</param>
        /// <returns></returns>
        Task<BXJGAttachmentCheckPermissionResult> CheckPermission(string module, BXJGFileOperation act, string permission);
    }
}
