using Abp.Application.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BXJG.GeneralTree
{
    public class GeneralTreeNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            var zcgl = new MenuItemDefinition(GeneralTreeConsts.GeneralTreeMenuName,
                                               "数据字典".L1(),
                                               icon: "generalTree",
                                               requiredPermissionName: GeneralTreeConsts.GeneralTreeGetPermissionName);
            context.Manager.MainMenu.AddItem(zcgl);
        }
    }
}
