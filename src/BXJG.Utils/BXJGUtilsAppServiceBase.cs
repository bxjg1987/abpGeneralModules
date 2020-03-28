using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Runtime.Session;

namespace BXJG.Utils
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class BXJGUtilsAppServiceBase : ApplicationService
    {
        //public TenantManager TenantManager { get; set; }

        //public UserManager UserManager { get; set; }

        protected BXJGUtilsAppServiceBase()
        {
            LocalizationSourceName = BXJGUtilsConsts.LocalizationSourceName;
        }

        //protected virtual async Task<User> GetCurrentUserAsync()
        //{
        //    var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
        //    if (user == null)
        //    {
        //        throw new Exception("There is no current user!");
        //    }

        //    return user;
        //}

        //protected virtual Task<Tenant> GetCurrentTenantAsync()
        //{
        //    return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        //}

        //protected virtual void CheckErrors(IdentityResult identityResult)
        //{
        //    identityResult.CheckErrors(LocalizationManager);
        //}
    }
}
