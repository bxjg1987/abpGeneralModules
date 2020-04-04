using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using System.IO;
using Microsoft.Extensions.Logging;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 微信支付结果通知处理中间件
    /// 拦截请求直接处理，不用进入“复杂的"mvc/webApi流程
    /// </summary>
    public class WeChatPaymentNoticeMiddleware
    {
        private readonly RequestDelegate _next;

        public WeChatPaymentNoticeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            //由于考虑用户代码的某些组件也需要访问此选项对象，因此使用asp.net core选项模型，使用依赖注入的方式
            //而不是使用注册中间件是直接提供Options的方式，这样调用方可以随时依赖注入此选项对象
            var options = context.RequestServices.GetRequiredService<IOptionsMonitor<WeChatPaymentOptions>>().CurrentValue;

            //若当前请求不是支付结果回调请求，则跳过处理，直接执行后续中间件
            if (options.CallbackPath != request.Path)
            {
                await this._next(context);
                return;
            }

            //另外将来可能使用GetServices拿到多个处理器，按顺序遍历，传入WeChatPaymentNoticeContext逐一处理 此情况目前不考虑
            var handler = context.RequestServices.GetService<IWeChatPaymentNoticeHandler>();
            if (handler == null)
            {
                await response.ResponseWeChatSuccessAsync();
                return;
            }

            var factory = context.RequestServices.GetRequiredService<WeChatPaymentNoticeResult.WeChatPaymentNoticeResultFactory>();
            var logger = context.RequestServices.GetService<ILogger>();

            //拿到微信传来的数据
            WeChatPaymentNoticeResult wpnr = await factory.LoadAsync(request.Body, context.RequestAborted);

            //无论微信通知我们支付成功还是失败都应该交给调用方去做业务处理

            var paymentNoticeContext = new WeChatPaymentNoticeContext(context, options, wpnr, logger);
            try
            {
                //中间件无法参与业务处理，因此业务处理需要考虑并发情况
                await handler.PaymentNoticeAsync(paymentNoticeContext);
                await response.ResponseWeChatSuccessAsync();
            }
            //catch (UserFriendlyException ex) {
            //  可以定义一个类似UserFriendlyException，让调用方的Handler返回业务异常是使用这个类。
            //  这里直接捕获，将ex.Message响应给微信，其它异常属于系统级别异常 就不要返回ex.Message给微信了
            //  await response.ResponseWeChatFailAsync(ex.Message);
            //}
            catch (Exception ex)
            {
                //这里可以做其它处理
                logger.LogError(ex, $"支付结果通知处理失败！微信支付订单号：{wpnr.transaction_id}");
                await response.ResponseWeChatFailAsync("业务处理失败！");
            }

        }
    }
}
