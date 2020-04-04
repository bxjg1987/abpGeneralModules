using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat.Payment
{
    public class WeChatPaymentConsts
    {
        public const string SUCCESS = "SUCCESS";
        public const string FAIL = "FAIL";
        /// <summary>
        /// 微信小程序支付中用来跟服务器通讯的HttpClient
        /// 参考 命名客户端：https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1
        /// </summary>
        public const string HttpClientName = "WeChatMiniProgramPayment";
        /// <summary>
        /// 微信小程序支付 统一下单接口地址
        /// </summary>
        public const string UnifyOrderUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";
        /// <summary>
        /// 微信支付成功 会请求我们的服务器以通知用户支付结果
        /// </summary>
        public const string PaymentNoticeCallbackPath = "/wechat-payment-notice";
    }
}
