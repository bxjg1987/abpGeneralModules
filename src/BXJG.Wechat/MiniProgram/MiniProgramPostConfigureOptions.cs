using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BXJG.WeChat.MiniProgram
{
    /*
     * 参考OAuthPostConfigureOptions<TOptions, THandler>
     * 
     * MiniProgramAuthenticationOptions的赋值有4个地方，先后顺序依次是：
     * 1、MiniProgramAuthenticationOptions的构造函数
     * 2、startup中注册身份验证方案时
     * 3、MiniProgramPostConfigureOptions
     * 4、MiniProgramAuthenticationHandler的构造函数
     * 
     * 有些参数的初始化比较复杂，最好是在用户设置后 判断如果用户未设置时做初始化，这种操作在步骤3比较合适
     * 个人感觉步骤3有点多此一举，完全可以用步骤4代替步骤3
     * 
     * 按微软的方式来实现，保持代码一致
     * 
     */
    public class MiniProgramPostConfigureOptions : IPostConfigureOptions<MiniProgramAuthenticationOptions>
    {
        private readonly IDataProtectionProvider _dp;

        public MiniProgramPostConfigureOptions(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }

        /// <summary>
        /// Invoked to post configure a TOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, MiniProgramAuthenticationOptions options)
        {
           // options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;

            //这段是推特的
            //if (options.StateDataFormat == null)
            //{
            //    var dataProtector = options.DataProtectionProvider.CreateProtector(
            //        typeof(TwitterHandler).FullName, name, "v1");
            //    options.StateDataFormat = new SecureDataFormat<RequestToken>(
            //        new RequestTokenSerializer(),
            //        dataProtector);
            //}

            //if (options.StateDataFormat == null)
            //{
            //    var dataProtector = options.DataProtectionProvider.CreateProtector(
            //        typeof(MiniProgramAuthenticationHandler).FullName, name, "v1");
            //    options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            //}

            if (options.Backchannel == null)
            {
                options.Backchannel = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler());
                options.Backchannel.Timeout = options.BackchannelTimeout;
                options.Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
                options.Backchannel.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                options.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Wechart miniprogram handler");
                options.Backchannel.DefaultRequestHeaders.ExpectContinue = false;
            }

        }
    }
}
