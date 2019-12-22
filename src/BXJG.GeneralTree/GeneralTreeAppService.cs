using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;

using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;
using Abp.Domain.Entities;
using Abp.MultiTenancy;

namespace BXJG.GeneralTree
{
    /*
     * 扩展方式一般有继承和组合，此处使用继承
     * Repository可以使用子类的或父类的
     * 由于父类存储Children属性，并且类型为父类的类型，因此AutoMapper映射时应该取消Children属性的映射，并手动处理
     * 也可以在子类实体覆盖Children属性
     * 
     * 新增时用管理类操作可以自动处理Code
     */
    //[AbpAuthorize(PermissionNames.AdministratorSystemAdministrative)]
    public class GeneralTreeAppService
        : GeneralTreeAppServiceBase< GeneralTreeEntity, GeneralTreeDto, GeneralTreeEditDt>,
        IGeneralTreeAppService
    {
        public GeneralTreeAppService(
            IRepository<GeneralTreeEntity, long> repository,
            GeneralTreeManager organizationUnitManager)
            : base(repository, organizationUnitManager)
        {
            base.createPermissionName = GeneralTreeConsts.GeneralTreeCreatePermissionName;
            base.updatePermissionName = GeneralTreeConsts.GeneralTreeUpdatePermissionName;
            base.deletePermissionName = GeneralTreeConsts.GeneralTreeDeletePermissionName;
            base.getPermissionName = GeneralTreeConsts.GeneralTreeGetPermissionName;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var sd = await base.ownRepository.GetAsync(input.Id);
            if (sd.IsSysDefine)
                throw new UserFriendlyException(L("系统预设数据不允许删除！"));

            await base.DeleteAsync(input);
        }
    }
}
