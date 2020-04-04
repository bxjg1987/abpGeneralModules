using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 微信小程序支付相关的选项对象
    /// </summary>
    public class WeChatPaymentOptions
    {
        /// <summary>
        /// 用户支付完，微信回调我方服务器的地址
        /// 默认：WeChatPaymentDefaults.PaymentNoticeCallbackPath
        /// </summary>
        public PathString CallbackPath { get; set; } =  WeChatPaymentConsts.PaymentNoticeCallbackPath;
        /// <summary>
        /// 商户id
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 微信商户平台(pay.weixin.qq.com)-->账户设置-->API安全-->密钥设置
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 注意与CallbackPath的关系
        /// http://xxxx.com/wechat-miniprogram-payment-notify
        /// </summary>
        public string notify_url { get; set; }
        /// <summary>
        /// 发起支付请求时要用(提交给微信)
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 获取或设置 微信小程序支付 统一下单接口地址
        /// </summary>
        public string UnifyOrderUrl { get; set; } = WeChatPaymentConsts.UnifyOrderUrl;

        //暂时不搞配置
        //public HttpClient Backchannel { get; set; }
    }
}
