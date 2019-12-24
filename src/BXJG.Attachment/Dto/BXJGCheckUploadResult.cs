using BXJG.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Attachment
{
    /// <summary>
    /// 文件上传前先做验证：权限、类型大小限制、和文件存在判断
    /// 此对象则是这个判断的返回值
    /// </summary>
    public class BXJGCheckUploadResult<TFileDto>
    {
        /// <summary>
        /// 判断结果状态
        /// </summary>
        public CheckUploadResultType State { get; set; }
        /// <summary>
        /// 对应的状态消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 若附件已存在则返回附件对象
        /// </summary>
        public TFileDto Data { get; set; }
    }
    /// <summary>
    /// 检查附件上传时的返回状态
    /// </summary>
    public enum CheckUploadResultType
    {
        /// <summary>
        /// 成功，文件不存在
        /// </summary>
        NotExists,
        /// <summary>
        /// 成功，文件已存在
        /// </summary>
        Exists,
        /// <summary>
        /// 不满足大小和类型限制
        /// </summary>
        Limit,
        /// <summary>
        /// 未授权
        /// </summary>
        Unauthorized
    }
}
