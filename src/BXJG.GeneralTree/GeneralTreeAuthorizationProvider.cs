using Abp.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    public class GeneralTreeAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var admin = context.CreatePermission(GeneralTreeConsts.GeneralTreeGetPermissionName, "数据字典".L1());
            admin.CreateChildPermission(GeneralTreeConsts.GeneralTreeCreatePermissionName, "新增".L1());
            admin.CreateChildPermission(GeneralTreeConsts.GeneralTreeUpdatePermissionName, "修改".L1());
            admin.CreateChildPermission(GeneralTreeConsts.GeneralTreeDeletePermissionName, "删除".L1());
        }
    }
}
