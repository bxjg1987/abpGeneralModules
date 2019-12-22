using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Attachment
{
    /// <summary>
    /// 上传文件前做判断的输入模型
    /// </summary>
    public class CheckUploadAttachmentInput
    {
        /// <summary>
        /// 附件模块名，如：EquipmentInfo
        /// </summary>
        [Required]
        public string Module { get; set; }
        /// <summary>
        /// 权限名，如：Administrator.Asset.EquipmentInfo.Create
        /// </summary>
        [Required]
        public string Permission { get; set; }
        /// <summary>
        /// 文件大小(kb)
        /// </summary>
        [Required]
        public long Size { get; set; }
        /// <summary>
        /// 文件后缀，如：.doc
        /// </summary>
        [Required]
        public string Extension { get; set; }
        ///// <summary>
        ///// 文件操作类型
        ///// </summary>
        //[Required]
        //public FileOperation FileOperation { get; set; }
        /// <summary>
        /// 文件的md5值
        /// </summary>
        [Required]
        public string MD5 { get; set; }
    }
}
