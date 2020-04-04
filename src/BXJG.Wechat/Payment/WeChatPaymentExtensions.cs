using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BXJG.WeChat.Payment
{
    public static class WeChatPaymentExtensions
    {
        #region 注册服务和中间件
        /// <summary>
        /// 向中间件管道注册微信小程序支付结果通知中间件
        /// 此中间件拦截微信小程序支付的结果通知，并从请求中获取相关数据进行封装，然后调用的的处理器(同时传入你需要的参数)
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWeChatPayment(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WeChatPaymentNoticeMiddleware>();
        }
        /// <summary>
        /// 向依赖注入容器添加微信小程序支付需要的相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWeChatPayment(this IServiceCollection services)
        {
            return services.AddWeChatPayment(opt => { });
        }
        /// <summary>
        /// 向依赖注入容器添加微信小程序支付需要的相关服务，并以单例形式注册THandler
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWeChatPayment<THandler>(this IServiceCollection services)
            where THandler : class, IWeChatPaymentNoticeHandler
        {
            return services.AddWeChatPayment<THandler>(opt => { });
        }
        /// <summary>
        /// 向依赖注入容器添加微信小程序支付需要的相关服务，并允许你通过act委托提供配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static IServiceCollection AddWeChatPayment(this IServiceCollection services, Action<WeChatPaymentOptions> act)
        {
            return services.AddWeChatPaymentCore().Configure(act);
        }
        /// <summary>
        /// 向依赖注入容器添加微信小程序支付需要的相关服务，并允许你通过act委托提供配置，并以单例形式注册THandler
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="services"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static IServiceCollection AddWeChatPayment<THandler>(this IServiceCollection services, Action<WeChatPaymentOptions> act)
             where THandler : class, IWeChatPaymentNoticeHandler
        {
            return services.AddWeChatPayment(act).AddSingleton<IWeChatPaymentNoticeHandler, THandler>();
        }
        /// <summary>
        /// 向依赖注入容器添加微信小程序支付需要的相关服务，并允许你通过configuration提供配置，通常你可以将此配置关联到配置文件(appsettings.json)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddWeChatPayment(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddWeChatPaymentCore().Configure<WeChatPaymentOptions>(configuration);
        }
        /// <summary>
        /// 向依赖注入容器添加微信小程序支付需要的相关服务，并允许你通过configuration提供配置，通常你可以将此配置关联到配置文件(appsettings.json)，并以单例形式注册THandler
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddWeChatPayment<THandler>(this IServiceCollection services, IConfiguration configuration)
             where THandler : class, IWeChatPaymentNoticeHandler
        {
            return services.AddWeChatPayment(configuration).AddSingleton<IWeChatPaymentNoticeHandler, THandler>();
        }

        //其实还可以提供一个默认配置节，对应到appsetting.json的指定节点

        /// <summary>
        /// 注册微信小程序支付的核心服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddWeChatPaymentCore(this IServiceCollection services)
        {
            services.AddHttpClient(WeChatPaymentConsts.HttpClientName, client =>
            {
                //微信小程序支付内部通过此httpClient来发起请求，这里可以统一对client进行配置
            });
            return services
                    .AddSingleton<WeChatPaymentService>()
                    //.AddSingleton<WeChatPaymentUnifyOrderInputFactory>()
                    .AddSingleton<WeChatPaymentSecuret>()
                    .AddSingleton<WeChatPaymentUnifyOrderResult.WeChatPaymentUnifyOrderResultFactory>()
                    .AddSingleton<WeChatPaymentNoticeResult.WeChatPaymentNoticeResultFactory>();
        }
        #endregion

        //public static Task ResponseWeChatAsync(this HttpContext context, string code, string msg = "")
        //{
        //    //将来可以考虑封装。
        //    //定义IWeChatResponseMessage 里面定义ToXML扩展方法，不同的消息有不同实现
        //    //可以进一步为HttpContext和HttpRequest提供扩展方法，简化调用
        //    //因为目前对微信整体开发了解不全面，不要盲目封装
        //    //var content = $@"<xml>
        //    //                    <return_code><![CDATA[{code}]]></return_code>
        //    //                    <return_msg><![CDATA[{msg}]]></return_msg>
        //    //                </xml>";
        //    //await context.Response.WriteAsync(content, context.RequestAborted);

        //    return context.Response.ResponseWeChatAsync(code, msg);
        //}
        public static async Task ResponseWeChatAsync(this HttpResponse response, string code, string msg = "")
        {
            //将来可以考虑封装。
            //定义IWeChatResponseMessage 里面定义ToXML扩展方法，不同的消息有不同实现
            //可以进一步为HttpContext和HttpRequest提供扩展方法，简化调用
            //因为目前对微信整体开发了解不全面，不要盲目封装
            var content = $@"<xml>
                                <return_code><![CDATA[{code}]]></return_code>
                                <return_msg><![CDATA[{msg}]]></return_msg>
                            </xml>";
            await response.WriteAsync(content, response.HttpContext.RequestAborted);
        }
        public static Task ResponseWeChatSuccessAsync(this HttpResponse response)
        {
            return response.ResponseWeChatAsync(WeChatPaymentConsts.SUCCESS, "OK");
        }
        public static Task ResponseWeChatFailAsync(this HttpResponse response, string msg)
        {
            return response.ResponseWeChatAsync(WeChatPaymentConsts.FAIL, msg);
        }
    }
}
