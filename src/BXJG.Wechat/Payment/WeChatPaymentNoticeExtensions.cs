using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat.Payment
{
    public static class WeChatPaymentNoticeExtensions
    {
        /// <summary>
        /// 向中间件管道注册微信支付结果通知中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWeChatPayment(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WeChatPaymentNoticeMiddleware>();
        }
        /// <summary>
        /// 向依赖注入容器添加支付功能需要的相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static IServiceCollection AddWeChatPayment(this IServiceCollection services, Action<WeChatPaymentOptions> act)
        {
            return services.Configure(act);
        }
        public static IServiceCollection AddWeChatPayment(this IServiceCollection services,IConfiguration configuration)
        {
            return services.Configure<WeChatPaymentOptions>(configuration);
        }
    }
}
