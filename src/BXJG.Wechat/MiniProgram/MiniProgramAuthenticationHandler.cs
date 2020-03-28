using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace BXJG.WeChat.MiniProgram
{
    //可以参考TwitterHandler和OAuthHandler的设计

    public class MiniProgramAuthenticationHandler : AuthenticationHandler<MiniProgramAuthenticationOptions>, IAuthenticationRequestHandler
    {
        private HttpClient Backchannel => Options.Backchannel;
        public MiniProgramAuthenticationHandler(IOptionsMonitor<MiniProgramAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// 实现IAuthenticationRequestHandler，若返回true则阻止后续中间件的执行，直接响应用户请求
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HandleRequestAsync()
        {
            if ((Options.CallbackPath != Request.Path))
                return false;

            var handler = Context.RequestServices.GetService<IWeChatMiniProgramLoginHandler>();
            //既然已经注册了这个身份验证方案，就必须提实现
            if (handler == null)
                throw new Exception($"请实现{nameof(IWeChatMiniProgramLoginHandler)}，否则不要注册微信小程序登录的身份验证方案");

            //在身份验证的多个步骤触发相应的事件，允许调用方控制身份验证过程，可以参考asp.net core默认实现，如：TwitterHandler
            //Options.Events.CreatingTicket(ct);

            var authResult = await HandleRemoteAuthenticateAsync();
            return await handler.ExcuteAsync(new WeChatMiniProgramLoginContext(base.Context, Options, authResult));
        }

        /// <summary>
        /// 从请求中获取用户上传信息
        /// 根据上传信息中的code向微信服务器发起请求，获取openid、session_key....
        /// </summary>
        /// <returns></returns>
        private async Task<WeChatUser> HandleRemoteAuthenticateAsync()
        {
            var input = await new StreamReader(Request.Body).ReadToEndAsync();
            var jd = JsonDocument.Parse(input).RootElement;

            var requestUrl = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, new Dictionary<string, string>
            {
                { "appid", Options.AppId },
                { "secret", Options.Secret },
                { "js_code",jd.GetString("code") },
                { "grant_type", "authorization_code" },
            });
            var response = await Backchannel.GetAsync(requestUrl, Context.RequestAborted);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"An error occurred when retrieving wechar mini program user information ({response.StatusCode}). Please check if the authentication information is correct and the corresponding Microsoft Account API is enabled.");

            //正式处理
            //var rt = JsonSerializer.Deserialize<MiniProgramToken>(await response.Content.ReadAsStringAsync());

            //调试
            var rt = new MiniProgramToken
            {
                errcode = 0,
                errmsg = "",
                openid = "33333",
                session_key = "222",
                unionid = ""
            };

            if (rt.errcode != 0)
                throw new HttpRequestException($"errcode:{rt.errcode}, errmsg:{rt.errmsg}");

            return new WeChatUser
            {
                Input = jd,
                openid = rt.openid,
                session_key = rt.session_key,
                unionid = rt.unionid
            };
        }

        /// <summary>
        /// 此方法的主要职责是从当前请求获取用户信息
        /// 本类中暂不实现，因此尝试使用类似HttpContext.SignInAsync("微信小程序登录");的方法时将引发异常
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
