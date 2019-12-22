using Abp.Authorization;
using Abp.Dependency;
using BXJG.File;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Attachment
{
    public class BXJGAttachmentPermissionChecker : IBXJGAttachmentPermissionChecker
    {
        //若是模块化开发，这里数据源的提供最好使用abp的模块配置模式实现
        //4.6.2 c#7.1 元组 https://docs.microsoft.com/zh-cn/dotnet/csharp/tuples#code-try-10
        //附件权限是依附于abp默认的权限判断的，它没必要提供动态设置功能，因此使用静态数据来定死

        //public static readonly IReadOnlyCollection<(string module, FileOperation act, string[] permissions)> Permissions;
        //static AttachmentPermissionChecker()
        //{
        //    var list = new List<(string, FileOperation, string[])> {
        //        (
        //            "EquipmentInfo",//这个最好改成常量
        //            FileOperation.Upload,
        //            new string[] {
        //                PermissionNames.AdministratorAssetEquipmentInfoCreate,
        //                PermissionNames.AdministratorAssetEquipmentInfoUpdate
        //            }
        //        )
        //    };
        //    Permissions = new ReadOnlyCollection<(string, FileOperation, string[])>(list);
        //}
        public static ICollection<BXJGAttachmentPermission> Permissions;
        //static BXJGAttachmentPermissionChecker()
        //{
        //    var list = new List<BXJGAttachmentPermission> {
        //        new BXJGAttachmentPermission(
        //            ABPConsts.AttachmentModuleName,
        //            FileOperation.Upload,
        //            new string[] {
        //                PermissionNames.AdministratorAssetEquipmentInfoCreate,
        //                PermissionNames.AdministratorAssetEquipmentInfoUpdate
        //            }
        //        ),
        //        new BXJGAttachmentPermission(
        //            ABPConsts.AttachmentModuleName,
        //            FileOperation.Download,
        //            new string[] {
        //                PermissionNames.AdministratorAssetEquipmentInfo
        //            }
        //        )
        //    };
        //    Permissions = new ReadOnlyCollection<BXJGAttachmentPermission>(list);
        //}


        protected readonly IPermissionChecker PermissionChecker;

        public BXJGAttachmentPermissionChecker(IPermissionChecker permissionChecker)
        {
            this.PermissionChecker = permissionChecker;
        }

        /// <summary>
        /// 验证附件操作权限
        /// </summary>
        /// <param name="module">模块名</param>
        /// <param name="act">附件动作，上传 下载 删除 修改(关键字，或手动修改物理文件刷新时)</param>
        /// <param name="permission">权限名</param>
        /// <returns></returns>
        public async Task<BXJGAttachmentCheckPermissionResult> CheckPermission(string module, BXJGFileOperation act, string permission)
        {
            var item = Permissions.SingleOrDefault(c => c.Module.Equals(module, StringComparison.OrdinalIgnoreCase) && c.Operation == act);

            if (item == null || !item.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase))
                return BXJGAttachmentCheckPermissionResult.IllegalRequest;

            if (!await PermissionChecker.IsGrantedAsync(permission))
                return BXJGAttachmentCheckPermissionResult.Unauthorized;

            return BXJGAttachmentCheckPermissionResult.Success;
        }
    }
}
