using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BXJG.WeChat.MiniProgram
{
    /// <summary>
    /// 参考Twitter中的AccessToken
    /// </summary>
    public class MiniProgramToken
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public string unionid { get; set; }
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }
}
