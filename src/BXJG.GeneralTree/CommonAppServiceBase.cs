using Abp.Application.Services;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLJ.Common;

namespace BXJG.GeneralTree
{
    public class CommonAppServiceBase<TTenant, TUser,TRole> : ApplicationService
        where TTenant : AbpTenant<TUser>
        where TRole : AbpRole<TUser>, new()
        where TUser : AbpUser<TUser>
    {
        public AbpTenantManager<TTenant, TUser> TenantManager { get; set; }

        public AbpUserManager<TRole, TUser> UserManager { get; set; }

        protected CommonAppServiceBase()
        {
            LocalizationSourceName = ZLJCommonConsts.LocalizationSourceName;
        }

        protected virtual Task<TUser> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual Task<TTenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
