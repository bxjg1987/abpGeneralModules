using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat
{
    /// <summary>
    /// 要向微信提交的数据
    /// </summary>
    public interface IPostToWeChat
    {
        string ToXml();
    }
}
