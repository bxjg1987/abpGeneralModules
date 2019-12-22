using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Attachment
{
    /// <summary>
    /// 验证附件操作权限时的返回结果
    /// </summary>
    public enum BXJGAttachmentCheckPermissionResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 非法请求
        /// </summary>
        IllegalRequest,
        /// <summary>
        /// 未授权
        /// </summary>
        Unauthorized
    }
}
