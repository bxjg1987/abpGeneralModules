using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    public abstract class GeneralTreeEntity<TEntity> : FullAuditedEntity<long>, IExtendableObject, IMayHaveTenant
        where TEntity : GeneralTreeEntity<TEntity>
    {
        public const int MaxDisplayNameLength = 128;
        public const int MaxDepth = 16;
        public const int CodeUnitLength = 5;
        public const int MaxCodeLength = 95;

        public GeneralTreeEntity() { }
        public GeneralTreeEntity(int? tenantId, string displayName, long? parentId = null)
        {
            this.TenantId = tenantId;
            this.DisplayName = displayName;
            this.ParentId = parentId;
        }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        public virtual long? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual TEntity Parent { get; set; }

        public virtual int? TenantId { get; set; }

        public virtual IList<TEntity> Children { get; set; }

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public string ExtensionData { get; set; }

        public string GetParentCode()
        {
            if (Parent != null)
                return Parent.Code;
            else
                return Code.Substring(0, Code.Length - GeneralTreeEntity.CodeUnitLength).TrimEnd('.');
        }
    }

    [Table("BXJGGeneralTrees")]
    public class GeneralTreeEntity : GeneralTreeEntity<GeneralTreeEntity>
    {
        public bool IsSysDefine { get; set; }
        public bool IsTree { get; set; }
    }
}
