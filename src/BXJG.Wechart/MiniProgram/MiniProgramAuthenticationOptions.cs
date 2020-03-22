using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BXJG.WeChart.MiniProgram
{
    /// <summary>
    /// 微信小程序身份验证方案关联的选项对象
    /// 参考OAuthOptions和MicrosoftAccountOptions
    /// 选项对象的初始化点有如下几个：
    /// 1、MiniProgramAuthenticationOptions的构造函数
    /// 2、startup中，调用方对象选项对象赋值
    /// 3、MiniProgramAuthenticationOptions
    /// 4、MiniProgramAuthenticationHandler的构造函数
    /// </summary>
    public class MiniProgramAuthenticationOptions : RemoteAuthenticationOptions
    {
        public MiniProgramAuthenticationOptions()
        {
            //小程序端向我方服务器发起登录时的请求地址
            CallbackPath = new PathString(MiniProgramConsts.CallbackPath);
            //我方服务器请求微信服务器获取openid、session_key的地址
            UserInformationEndpoint = MiniProgramConsts.UserInformationEndpoint;
            //我方服务器请求微信服务器获取openid、session_key的超时时间，默认30秒
            BackchannelTimeout = TimeSpan.FromSeconds(30);
            //身份验证处理器执行过程中的回调函数，由调用方在startup中配置小程序登录时设置
            Events = new MiniProgramAuthenticationEvent();

            //此处定义的是想存储到第三方登录的额外字段，必须有的字段在handler中处理
            //ClaimActions.MapJsonKey("openid", "openid");
            //ClaimActions.MapJsonKey("session_key", "session_key");
            //ClaimActions.MapJsonKey("unionid", "unionid");

            //ClaimActions.MapCustomJson(ClaimTypes.Email, user => user.GetString("mail") ?? user.GetString("userPrincipalName"));
        }

        /// <summary>
        /// 验证选项对象
        /// </summary>
        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(AppId))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrEmpty(Secret))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrEmpty(UserInformationEndpoint))
            {
                throw new ArgumentException();
            }
            if (!CallbackPath.HasValue)
            {
                throw new ArgumentException();
            }
        }

        public ClaimActionCollection ClaimActions { get; } = new ClaimActionCollection();

        public string UserInformationEndpoint { get; set; }

        public string AppId { get; set; }

        public string Secret { get; set; }

        //public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

        public new MiniProgramAuthenticationEvent Events
        {
            get => (MiniProgramAuthenticationEvent)base.Events;
            set => base.Events = value;
        }
    }
}
