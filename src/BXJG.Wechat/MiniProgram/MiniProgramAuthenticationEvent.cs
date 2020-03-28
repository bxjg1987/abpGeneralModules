using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.WeChat.MiniProgram
{
    /// <summary>
    /// 此对象没啥用，主要用来演示自定义身份验证方案如何实现事件方式，以方便调用方控制身份验证过程
    /// </summary>
    public class MiniProgramAuthenticationEvent //: RemoteAuthenticationEvents
    {
     // public Func<MiniProgramCreatingTicketContext, Task> OnCreatingTicket { get; set; } = context => Task.CompletedTask;
       // public virtual Task CreatingTicket(MiniProgramCreatingTicketContext context) => OnCreatingTicket(context);
    }
}
