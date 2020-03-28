using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat.MiniProgram
{
    /*
     * 参考TwitterExtensions
     */
    public static class MiniProgramAuthencationExtensions
    {
        public static AuthenticationBuilder AddWeChartMiniProgram<TLoginHandler>(this AuthenticationBuilder builder) 
            where TLoginHandler: class, IWeChatMiniProgramLoginHandler
            => builder.AddWeChartMiniProgram<TLoginHandler>(MiniProgramConsts.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddWeChartMiniProgram<TLoginHandler>(this AuthenticationBuilder builder, Action<MiniProgramAuthenticationOptions> configureOptions) 
            where TLoginHandler : class, IWeChatMiniProgramLoginHandler
            => builder.AddWeChartMiniProgram<TLoginHandler>(MiniProgramConsts.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddWeChartMiniProgram<TLoginHandler>(this AuthenticationBuilder builder, string authenticationScheme, Action<MiniProgramAuthenticationOptions> configureOptions) 
            where TLoginHandler : class, IWeChatMiniProgramLoginHandler
            => builder.AddWeChartMiniProgram<TLoginHandler>(authenticationScheme, MiniProgramConsts.AuthenticationSchemeDisplayName, configureOptions);

        public static AuthenticationBuilder AddWeChartMiniProgram<TLoginHandler>(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<MiniProgramAuthenticationOptions> configureOptions) 
            where TLoginHandler :class, IWeChatMiniProgramLoginHandler
        {
            builder.Services.AddScoped<IWeChatMiniProgramLoginHandler, TLoginHandler>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<MiniProgramAuthenticationOptions>, MiniProgramPostConfigureOptions>());
            return builder.AddScheme<MiniProgramAuthenticationOptions, MiniProgramAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
