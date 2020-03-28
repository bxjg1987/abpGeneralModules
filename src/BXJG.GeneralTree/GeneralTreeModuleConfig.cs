using Abp.Application.Navigation;
using Abp.Authorization;
using BXJG.Utils.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    /// <summary>
    /// 公共模块配置对象
    /// </summary>
    public class GeneralTreeModuleConfig
    {
        ///// <summary>
        ///// 是否启用通用树动态webApi注册
        ///// 若开启，则根据通用树应用服务接口自动生成动态WebApi
        ///// 且您应该在您的AuthorizationProvider中定义权限，权限名定义在GeneralTreeConsts中
        ///// </summary>
        //public bool EnableGeneralTreeDynamicWebApi { get; set; } = true;
        private Permission InitPermission1(Permission admin)
        {
            admin.CreateChildPermission(GeneralTreeConsts.GeneralTreeCreatePermissionName, "新增".UtilsLI());
            admin.CreateChildPermission(GeneralTreeConsts.GeneralTreeUpdatePermissionName, "修改".UtilsLI());
            admin.CreateChildPermission(GeneralTreeConsts.GeneralTreeDeletePermissionName, "删除".UtilsLI());
            return admin;
        }
        public Permission InitPermission(IPermissionDefinitionContext context)
        {
            Permission admin = context.CreatePermission(GeneralTreeConsts.GeneralTreeGetPermissionName, "数据字典".GeneralTreeLI());
            return InitPermission1(admin);
        }
        public Permission InitPermission(Permission parent)
        {
            var admin = parent.CreateChildPermission(GeneralTreeConsts.GeneralTreeGetPermissionName, "数据字典".GeneralTreeLI());
            return InitPermission1(admin);
        }


        public MenuItemDefinition InitNav(MenuItemDefinition menuItemDefinition)
        {
            var zcgl = new MenuItemDefinition(GeneralTreeConsts.GeneralTreeMenuName,
                                                  "数据字典".GeneralTreeLI(),
                                                  icon: "generalTree",
                                                  permissionDependency: new SimplePermissionDependency(GeneralTreeConsts.GeneralTreeGetPermissionName));
            menuItemDefinition.AddItem(zcgl);
            return zcgl;
        }
     
    }
}
