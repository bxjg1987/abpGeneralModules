using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.File
{
    /// <summary>
    /// 文件
    /// </summary>
    public class BXJGFileBaseEntty : FullAuditedEntity<long>//, IMustHaveTenant 同一个文件可以被所有租户引用，至于能否访问是有文件的依赖方决定的
    {

        ///// <summary>
        ///// 租户Id
        ///// </summary>
        //public int TenantId { get; set; }//附件是跟随实体走的，文件不一定一定要依附于实体
        /// <summary>
        /// 文件名
        /// </summary>
        [StringLength(BXJGFileConsts.FileNameMaxLength)]
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 助记码
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(BXJGFileConsts.FileMnemonicCodeMaxLength)]
        public string MnemonicCode { get; set; }
        //记得建立唯一索引，在Ef项目里
        /// <summary>
        /// 文件的md5值
        /// </summary>
        [Column(TypeName = "char")]
        [StringLength(BXJGFileConsts.FileMD5MaxLength)]
        public string MD5 { get; set; }
        /// <summary>
        /// 文件大小kb
        /// </summary>
        [Required]
        public long Size { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [StringLength(BXJGFileConsts.FileKeywordsMaxLength)]
        public string Keywords { get; set; }
        ///// <summary>
        ///// 0
        ///// </summary>
        //public int ConsistencyState { get; set; }
        /// <summary>
                                         /// 获取文件扩展名，如：.txt
                                         /// </summary>
        public string Extension
        {
            //本来应该定义为GetExtension()
            get { return System.IO.Path.GetExtension(this.Path); }
        }
        /// <summary>
        /// 获取MIME类型(根据Extension获取)
        /// </summary>
        public string Mime
        {
           // GetMime()
            get
            {
                return MimeMapping.MimeUtility.GetMimeMapping(this.Extension);
            }
        }
        /// <summary>
        /// 获取文件的物理路径
        /// </summary>
        /// <summary>
        /// 存储路径，不包含BXJGConsts.UploadRoot及其之前的路径。注：windows路径大多是反斜杠，如：employee\xxx.jpg
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(BXJGFileConsts.FilePathMaxLength)]
        public string Path { get; set; }
        /// <summary>
        /// 获取访问的相对路径，如：upload\employeeinfo\xxx.jpg
        /// </summary>
        public string RelativePath
        {
            //本来应该定义为GetRelativePath()
            get
            {
                return System.IO.Path.Combine(BXJGFileConsts.UploadRoot, this.Path);
            }
        }
        /// <summary>
        /// 获取服务器上的物理路径，如：D:\www\abp\upload\employeeinfo\xxx.jpg
        /// </summary>
        public string AbsolutePath
        {
            //本来应该定义为GetAbsolutePath()
            get
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.RelativePath);
            }
        }
        /*
         * 定时任务尝试删除每一个文件
         * 那些被引用的文件由于有外键约束删除会失败
         * 如果一个文件刚被上传或获取，还没来得及被引用时可能被删除，因此通过ReferenceLastTime来控制，最近可能被引用的文件不要被删除
         */
        ///// <summary>
        ///// 最后被引用(访问)时间
        ///// 控制定时任务的删除
        ///// </summary>
        //public DateTime ReferenceLastTime { get; set; }
        ///// <summary>
        ///// 刷新最后访问时间
        ///// </summary>
        //public void RefresReference()
        //{
        //    ReferenceLastTime = DateTime.Now;
        //}
        /// <summary>
        /// 同步物理文件信息到实体对象上
        /// </summary>
        public void RefreshAsync()
        {
            var fileInfo = new FileInfo(AbsolutePath);
            MD5 = AbsolutePath.GetMD5ByFilePath();
            Size = fileInfo.Length;
            LastModificationTime = fileInfo.LastWriteTime;
            //ReferenceLastTime = DateTime.Now;
        }
        /// <summary>
        /// 获取文件流以供读取
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            //return File.OpenRead(AbsolutePath);
            return System.IO.File.Open(AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        //反正每个有用的文件始终会被引用，因此通过数据库外键方式来做定时任务的删除依据
        //因为使用这种方式 需要自己来做乐观并发
        ///// <summary>
        ///// 此文件被引用的次数
        ///// </summary>
        //[ConcurrencyCheck]//若控制不了删除，则用TimeStamp
        //public long ReferenceCount { get; set; }
    }

    /// <summary>
    /// 文件
    /// </summary>
    [Table("BXJGFiles")]
    public class BXJGFileEntty : BXJGFileBaseEntty
    { 
    
    }
}
