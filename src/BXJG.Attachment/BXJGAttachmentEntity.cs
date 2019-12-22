using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BXJG.File;
namespace BXJG.Attachment
{
    /// <summary>
    /// 附件
    /// </summary>
    public class BXJGAttachmentEntity<TFile> : Entity<long> //FullAuditedEntity<long>, IMustHaveTenant 附件是跟着实体走的，没必要用这俩
        where TFile:BXJGFileEntty
    {
        ///// <summary>
        ///// 租户Id
        ///// </summary>
        //public int TenantId { get; set; }
        /// <summary>
        /// 关联的文件
        /// </summary>
        public TFile File { get; set; }
        /// <summary>
        /// 关联的文件Id
        /// </summary>
        public long FileId { get; set; }
        /// <summary>
        /// 附件所属模块
        /// 这个并不是指abp的模块，而是通用附件中功能里的模块，如设备档案:equipmentinfo 会议信息：huiyi）
        /// 如果是“文档管理”模块的文件，此属性和ObjectId应该为空
        /// 如果后期考虑一个附件被多个实体引用，可以考虑移除这俩属性 并单独建立一张多对多关系的表
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(BXJGAttachmentConsts.AttachmentModuleMaxLength)]
        public string Module { get; set; }
        /// <summary>
        /// 所属模块下的对象的id
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(BXJGAttachmentConsts.AttachmentObjectIdMaxLength)]
        public string ObjectId { get; set; }
    }
   
    [Table("BXJGAttachments")]
    public class BXJGAttachmentEntity : BXJGAttachmentEntity<BXJGFileEntty>
    { }
}
