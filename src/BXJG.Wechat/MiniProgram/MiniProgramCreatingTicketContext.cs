using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BXJG.WeChat.MiniProgram
{
    /// <summary>
    /// 从微信获取openid、session_key后会创建identity，然后回调Opetions.ClaimActions进行identity.Claims赋值
    /// 最后回调Options.Event个调用方一个机会修改或向Ticket添加自定义数据，此处的上下文对象就是这个事件回调的上下文对象
    /// 参考asp.net core 3.1源码定义的MiniProgramAuthenticationOptions或OAuthCreatingTicketContext
    /// </summary>
    public class MiniProgramCreatingTicketContext : ResultContext<MiniProgramAuthenticationOptions>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">当前请求上下文</param>
        /// <param name="scheme">当前身份验证方案名，默认MiniProgramConsts.AuthenticationScheme</param>
        /// <param name="options">当前身份验证方案关联的选项对象</param>
        /// <param name="principal">正在构建的用户(主体)</param>
        /// <param name="properties">身份验证关联的额外数据</param>
        /// <param name="openId">小程序对接的openid</param>
        /// <param name="session_key">小程序对接的session_key</param>
        /// <param name="unionid">小程序对接的unionid</param>
        /// <param name="user">包含小程序用户的所有信息</param>
        public MiniProgramCreatingTicketContext(
            HttpContext context,
            AuthenticationScheme scheme,
            MiniProgramAuthenticationOptions options,
            ClaimsPrincipal principal,
            AuthenticationProperties properties,
            string openId,
            string session_key,
            string unionid,
            JsonElement user)
            : base(context, scheme, options)
        {
            this.openId = openId;
            this.session_key = session_key;
            this.unionid = unionid;
            User = user;
            Principal = principal;
            Properties = properties;
        }

        public string openId { get;  }

        public string session_key { get;  }
        public string unionid { get;  }
        /// <summary>
        /// Gets the JSON-serialized user or an empty
        /// <see cref="JsonElement"/> if it is not available.
        /// </summary>
        public JsonElement User { get; }
        public ClaimsIdentity Identity => Principal?.Identity as ClaimsIdentity;
    
        public void RunClaimActions() => RunClaimActions(User);

        public void RunClaimActions(JsonElement userData)
        {
            foreach (var action in Options.ClaimActions)
            {
                action.Run(userData, Identity, Options.ClaimsIssuer ?? Scheme.Name);
            }
        }
    }
}
