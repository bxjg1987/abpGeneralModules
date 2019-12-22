using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    /// <summary>
    /// 通用树形数据更新模型
    /// </summary>
    [AutoMapTo(typeof(GeneralTreeEntity))]
    public class GeneralTreeEditDt : GeneralTreeNodeEditBaseDto
    {
        public bool IsTree { get; set; } = false;
    }

}
