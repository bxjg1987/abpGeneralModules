using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat.Payment
{
    public class WeChatPaymentNoticeContext
    {
        public HttpContext HttpContext { get; }
        public WeChatPaymentNoticeOptions Options { get; }
        public WeChatPaymentNoticeResult PaymentNoticeResult { get; }

        public WeChatPaymentNoticeContext(HttpContext httpContext, WeChatPaymentNoticeOptions options, WeChatPaymentNoticeResult paymentNoticeResult)
        {
            HttpContext = httpContext;
            Options = options;
            PaymentNoticeResult = paymentNoticeResult;
        }
    }
}
