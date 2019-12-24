using Abp;
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
    /*
     * 默认的实现思路：
     * 1、由调用方来设置BXJGAttachmentPermission（附件权限定义）集合
     * 2、默认实现内部每次验证时从上面的集合中获取附件权限定义，然后匹配进行验证
     * 
     * 向调用方提供注册权限的方式可以有很多种方式
     * 1、类似Abp提供的PermissionProvider，可以定义一个IBXJGAttachmentPermissionProvider接口；
     * 然后在BXJGAttachmentModuleConfig中定义api允许调用方注册自己的BXJGAttachmentPermissionProvider
     * 2、更简单的方式是通过BXJGAttachmentModuleConfig提供api直接定义BXJGAttachmentPermission，目前使用这种方式，参考BXJGAttachmentModuleConfig
     * 
     * 附件权限是依附于abp默认的权限判断的，它没必要提供动态设置功能，因此使用静态数据来定死
     *  
     * 参考：c#7.1 元组 https://docs.microsoft.com/zh-cn/dotnet/csharp/tuples#code-try-10
     */

    /// <summary>
    /// 附件权限验证器的默认实现
    /// 在使用此接口之前应在你的模块中先配置附件权限，参考：IModuleConfigurations.BXJGAttachmentModuleConfig()扩展方法
    /// </summary>
    public class BXJGAttachmentPermissionChecker : IBXJGAttachmentPermissionChecker
    {
        //权限定义已经移动到BXJGAttachmentModuleConfig
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
        //public static ICollection<BXJGAttachmentPermission> Permissions;
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

        readonly IPermissionChecker PermissionChecker;

        readonly BXJGAttachmentModuleConfig cfg;

        public BXJGAttachmentPermissionChecker(IPermissionChecker permissionChecker, BXJGAttachmentModuleConfig bXJGAttachmentModuleConfig)
        {
            this.PermissionChecker = permissionChecker;
            this.cfg = bXJGAttachmentModuleConfig;
        }

        public async Task<BXJGAttachmentCheckPermissionResult> CheckPermissionAsync(string module, BXJGFileOperation act, string permission)
        {
            //var item = Permissions.SingleOrDefault(c => c.Module.Equals(module, StringComparison.OrdinalIgnoreCase) && c.Operation == act);

            //if (item == null || !item.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase))
            //    return BXJGAttachmentCheckPermissionResult.IllegalRequest;
            if (IsIllegalRequest(module, act, permission))
                return BXJGAttachmentCheckPermissionResult.IllegalRequest;

            if (!await PermissionChecker.IsGrantedAsync(permission))
                return BXJGAttachmentCheckPermissionResult.Unauthorized;

            return BXJGAttachmentCheckPermissionResult.Success;
        }

        public async Task<BXJGAttachmentCheckPermissionResult> CheckPermissionAsync(UserIdentifier userIdentifier, string module, BXJGFileOperation act, string permission)
        {
            if (IsIllegalRequest(module, act, permission))
                return BXJGAttachmentCheckPermissionResult.IllegalRequest;

            if (!await PermissionChecker.IsGrantedAsync(userIdentifier, permission))
                return BXJGAttachmentCheckPermissionResult.Unauthorized;

            return BXJGAttachmentCheckPermissionResult.Success;
        }
        /// <summary>
        /// 判断是否是非法请求
        /// </summary>
        /// <param name="module"></param>
        /// <param name="act"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool IsIllegalRequest(string module, BXJGFileOperation act, string permission)
        {
            BXJGAttachmentPermission item = cfg.Permissions.SingleOrDefault(c => c.Module.Equals(module, StringComparison.OrdinalIgnoreCase) && c.Operation == act);

            return item == null || !item.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }
    }
}
