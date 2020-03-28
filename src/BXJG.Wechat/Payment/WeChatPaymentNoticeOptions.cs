using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat.Payment
{
    public class WeChatPaymentNoticeOptions
    {
        /// <summary>
        /// 用户支付完，微信回调我方服务器的地址
        /// 默认：WeChatPaymentDefaults.PaymentNoticeCallbackPath
        /// </summary>
        public string CallbackPath { get; set; } = WeChatPaymentDefaults.PaymentNoticeCallbackPath;
    }
}
