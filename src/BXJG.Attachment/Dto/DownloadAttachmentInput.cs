using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Attachment
{
    /*
     * 由于上传时需要的是文件id
     * 下载的时候最好是由调用方提供附件id
     * 但是这样比较麻烦，因为文件必须与实体关联后才能形成附件id
     * 所以在修改实体时必须返回附件id和文件id，因为点击保存时需要重新设置整个实体的附件
     * 最终决定统一成文件id，需要附件时提供Module
     * 
     * 算了吧还是分开整。。。。
     */

    /// <summary>
    /// 下载附件时的输入模型
    /// </summary>
    public class DownloadAttachmentInput
    {
        /// <summary>
        /// 权限名，如：Administrator.Asset.EquipmentInfo.Create
        /// </summary>
        [Required]
        public string Permission { get; set; }
        ///// <summary>
        ///// 模块名
        ///// </summary>
        //[Required]
        //public string Module { get; set; }
        /// <summary>
        /// 要下载的附件的id
        /// </summary>
        [Required]
        public long Id { get; set; }
    }
}
