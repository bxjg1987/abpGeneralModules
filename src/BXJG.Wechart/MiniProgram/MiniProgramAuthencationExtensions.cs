using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChart.MiniProgram
{
    /*
     * 参考TwitterExtensions
     */
    public static class MiniProgramAuthencationExtensions
    {
        public static AuthenticationBuilder AddWeChartMiniProgram(this AuthenticationBuilder builder)
            => builder.AddWeChartMiniProgram(MiniProgramConsts.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddWeChartMiniProgram(this AuthenticationBuilder builder, Action<MiniProgramAuthenticationOptions> configureOptions)
            => builder.AddWeChartMiniProgram(MiniProgramConsts.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddWeChartMiniProgram(this AuthenticationBuilder builder, string authenticationScheme, Action<MiniProgramAuthenticationOptions> configureOptions)
            => builder.AddWeChartMiniProgram(authenticationScheme, MiniProgramConsts.AuthenticationSchemeDisplayName, configureOptions);

        public static AuthenticationBuilder AddWeChartMiniProgram(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<MiniProgramAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<MiniProgramAuthenticationOptions>, MiniProgramPostConfigureOptions>());
            return builder.AddRemoteScheme<MiniProgramAuthenticationOptions, MiniProgramAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
