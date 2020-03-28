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
using BXJG.Utils.XML;


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

            //若考虑配置文件更新时自动更新选项，则使用IOptionsMonitor<>.CurrentValue，否则用IOptions<>.Value
            //由于考虑用户代码的某些组件也需要访问此选项对象，因此使用asp.net core选项模型，使用依赖注入的方式
            //而不是使用注册中间件是直接提供Options的方式，这样调用方可以随时依赖注入此选项对象
            var options = context.RequestServices.GetRequiredService<IOptionsMonitor<WeChatPaymentNoticeOptions>>().CurrentValue;

            //若当前请求不是支付结果回调请求，则跳过处理，直接执行后续中间件
            if (!options.CallbackPath.Equals(request.Path, StringComparison.OrdinalIgnoreCase))
            {
                await this._next(context);
                return;
            }

            //找到处理器，若没有则直接相应成功
            //另外将来可能使用GetServices拿到多个处理器，按顺序遍历，传入WeChatPaymentNoticeContext逐一处理
            //最后通过返回值WeChatPamentNoticeHandleResult来判断成功与否
            //这个目前不考虑实现
            var handler = context.RequestServices.GetService<IWeChatPaymentNoticeHandler>();
            if (handler == null)
            {
                await response.ResponseWeChatSuccessAsync();
                return;
            }

            var logger = context.RequestServices.GetService<ILogger>();

            //拿到微信传来的数据
            //没必要先判断return_code再决定是否序列化，因为大部分时候都是成功的，先判断还得linq to xml 的多创建一个XDocument，没必要
            var wpnr = await request.Body.XmlDeserializeAsync<WeChatPaymentNoticeResult>();

            //如果微信传来的状态就是错误的就直接返回
            //这里也可以让调用方去执行一些业务
            if (!wpnr.return_code.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
            {
                await response.ResponseWeChatFailAsync(wpnr.return_msg);
                return;
            }

            var paymentNoticeContext = new WeChatPaymentNoticeContext(context, options, wpnr);
            try
            {
                //对于微信来说只需要知道成功或失败，失败时的原因
                //内部处理有两种异常，一种是由中间件直接去异常消息返回给微信
                //另外可能是系统级异常，比如空引用，没必要返回给微信，由中间件统一处理。也可以考虑允许调用方进行配置
                //仔细考虑过 中间件不容易处理并发问题，因为中间件的处理方式无非跟微信的方式一样，多次尝试，不可能你一直不告诉我上一次的结果我就一直不重试
                //一旦我发起重试你同样可能出现并发问题。
                await handler.PaymentNoticeAsync(paymentNoticeContext);
            }
            catch (Exception ex)
            {
                //这里可以做其它处理
                logger.LogError($"支付结果通知处理失败！{ex.Message}，微信支付订单号：{wpnr.transaction_id}");
                await response.ResponseWeChatFailAsync("签名失败");
            }
        }
    }
}
