using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    /// <summary>
    /// 数据字典管理
    /// </summary>
    public interface IGeneralTreeAppService : IGeneralTreeAppServiceBase<GeneralTreeDto, GeneralTreeEditDt>
    {
       
    }
}
