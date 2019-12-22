using BXJG.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Attachment
{
    /// <summary>
    /// 下载附件的返回值
    /// </summary>
    public class DownloadOutput:BXJGFileDto
    {
        public long AttachmentId { get; set; }
        public Stream Stream { get; set; }
    }
}
