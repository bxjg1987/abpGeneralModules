using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BXJG.File;
namespace BXJG.Attachment
{
    /// <summary>
    /// 获取实体dto时附带的附件集合的项
    /// </summary>
    public class BXJGAttachmentDto : BXJGFileDto
    {
        /// <summary>
        /// 附件id
        /// </summary>
        public long AttachmentId { get; set; }
    }
}
