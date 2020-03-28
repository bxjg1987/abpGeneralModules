using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat.MiniProgram
{
    /// <summary>
    /// 调用方的实现IWeChatMiniProgramLoginHandler时会使用到的上下文对象
    /// 具体包含哪些内容请看定义
    /// </summary>
    public class WeChatMiniProgramLoginContext
    {
        public HttpContext HttpContext { get; }
        //实现类本身可以通过选项模式访问到这个对象，不过这里提供会让实现方更方便
        public MiniProgramAuthenticationOptions Options { get; }
        public WeChatUser WeChatMiniProgramUser { get; }
        public WeChatMiniProgramLoginContext(HttpContext httpContext, MiniProgramAuthenticationOptions options, WeChatUser weChatMiniProgramUser)
        {
            HttpContext = httpContext;
            Options = options;
            WeChatMiniProgramUser = weChatMiniProgramUser;
        }
    }
}
