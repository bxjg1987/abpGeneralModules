using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.WeChat.MiniProgram
{
    /// <summary>
    /// 当小程序登录身份验证中间件获取到微信用户信息(openid、session_key)后将调用此接口并传递包含微信用户信息的上下文
    /// 您应该实现此接口来完成小程序登录的后续逻辑
    /// </summary>
    public interface IWeChatMiniProgramLoginHandler
    {
        /// <summary>
        /// 请先查看接口注释
        /// 通常应该最后使用ct.HttpContext来响应用户请求，比如：响应jwtToken
        /// </summary>
        /// <param name="ct">包含微信用户信息和其它相关信息的上下文对象</param>
        /// <returns>返回true将阻止后续中间件执行，否则将继续执行后续中间件</returns>
        Task<bool> ExcuteAsync(WeChatMiniProgramLoginContext ct);
    }
}
