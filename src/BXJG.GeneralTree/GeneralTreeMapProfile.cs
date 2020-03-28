using System.Linq;
using AutoMapper;
using Abp.Authorization;
using Newtonsoft.Json;
using BXJG.Utils.Localization;

namespace BXJG.GeneralTree
{
    public class GeneralTreeMapProfile : Profile
    {
        public GeneralTreeMapProfile()
        {
            CreateMap<GeneralTreeEntity, GeneralTreeDto>()
                .ForMember(c => c.ExtData, opt => opt.MapFrom(c => JsonConvert.DeserializeObject<dynamic>(c.ExtensionData)))
                .ForMember(c => c.Children, opt => opt.Ignore());
            //.ForMember(c => c.IsTreeText, opt => opt.MapFrom(c => c.IsTree.ToString().UtilsL()))
            //.ForMember(c => c.IsSysDefineText, opt => opt.MapFrom(c => c.IsSysDefine.ToString().UtilsL()))

            //CreateMap<GeneralTreeEditDto, GeneralTreeEntity>()
            //  .

            //// Role and permission
            //CreateMap<Permission, string>().ConvertUsing(r => r.Name);
            //CreateMap<RolePermissionSetting, string>().ConvertUsing(r => r.Name);

            //CreateMap<CreateRoleDto, Role>();

            //CreateMap<RoleDto, Role>();

            //CreateMap<Role, RoleDto>().ForMember(x => x.GrantedPermissions,
            //    opt => opt.MapFrom(x => x.Permissions.Where(p => p.IsGranted)));

            //CreateMap<Role, RoleListDto>();
            //CreateMap<Role, RoleEditDto>();
            //CreateMap<Permission, FlatPermissionDto>();
        }
    }
}
