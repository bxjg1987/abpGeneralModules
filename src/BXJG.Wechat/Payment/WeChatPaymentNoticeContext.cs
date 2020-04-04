using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 包含微信小程序支付结果通知的数据的上下文对象
    /// </summary>
    public class WeChatPaymentNoticeContext
    {
        public HttpContext HttpContext { get; }
        public WeChatPaymentOptions Options { get; }
        public WeChatPaymentNoticeResult PaymentNoticeResult { get; }
        public ILogger Logger { get; }

        //logger属性本来应该考虑空模式
        //Options和Logger 本身不必要提供的，库的调用方完全可以自己做依赖注入。但是由于中间件中已经拿到这些对象的引用，直接传递过来 比去依赖注入再拿一次 更快吧

        public WeChatPaymentNoticeContext(HttpContext httpContext, WeChatPaymentOptions options, WeChatPaymentNoticeResult paymentNoticeResult, ILogger logger)
        {
            HttpContext = httpContext;
            Options = options;
            PaymentNoticeResult = paymentNoticeResult;
            Logger = logger;
        }
    }
}
