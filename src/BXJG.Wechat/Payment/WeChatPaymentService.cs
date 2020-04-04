using BXJG.WeChat.MiniProgram;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BXJG.WeChat.Payment.WeChatPaymentUnifyOrderResult;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 微信小程序支付服务类
    /// </summary>
    public class WeChatPaymentService
    {
        private readonly MiniProgramAuthenticationOptions authOptions;
        private readonly WeChatPaymentOptions paymentOptions;
        private readonly WeChatPaymentUnifyOrderResultFactory weChatPaymentUnifyOrderResultFactory;
        //按asp.net core 身份验证系统的设计，它的OAuth2的方式会允许Options对象来提供HttpClient，我们这里为了简单直接注入IHttpClientFactory
        //参考官网HttpClientFacotry
        private readonly IHttpClientFactory clientFactory;

        private readonly WeChatPaymentSecuret securet;


        public WeChatPaymentService(
              IOptionsMonitor<WeChatPaymentOptions> paymentOptions,
              IOptionsMonitor<MiniProgramAuthenticationOptions> authOptions,
              WeChatPaymentUnifyOrderResultFactory weChatPaymentUnifyOrderResultFactory,
              IHttpClientFactory clientFactory,
              WeChatPaymentSecuret securet)
        {
            this.paymentOptions = paymentOptions.CurrentValue;
            this.authOptions = authOptions.CurrentValue;
            this.weChatPaymentUnifyOrderResultFactory = weChatPaymentUnifyOrderResultFactory;
            this.clientFactory = clientFactory;
            this.securet = securet;
        }
        /// <summary>
        /// 调用微信发起统一下单
        /// 在此之前你应该调用Create方法来创建要提交的数据（参数data）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WeChatPaymentUnifyOrderResult> PayAsync(WeChatPaymentUnifyOrderInput data, CancellationToken cancellationToken = default)
        {
            var client = clientFactory.CreateClient(WeChatPaymentConsts.HttpClientName);
            var str = data.ToXml();
            var ct = new StringContent(str);
            var response = await client.PostAsync(paymentOptions.UnifyOrderUrl, ct, cancellationToken);
            return await weChatPaymentUnifyOrderResultFactory.LoadAsync(await response.Content.ReadAsStreamAsync(), cancellationToken);
        }

        /// <summary>
        /// 创建统一下单时要准备提交的数据
        /// 参考WeChatPaymentUnifyOrderInputFactory的备注
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="body">商品简单描述，该字段请按照规范传递 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2 </param>
        /// <param name="out_trade_no">商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-|*且在同一个商户号下唯一 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2 </param>
        /// <param name="total_fee">订单总金额，单位为分 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2 </param>
        /// <returns></returns>
        public WeChatPaymentUnifyOrderInput Create(
            string body,
            string out_trade_no,
            decimal total_fee)
        {
            return new WeChatPaymentUnifyOrderInput(
                securet,
                authOptions.AppId,
                paymentOptions.mch_id,
                paymentOptions.notify_url,
                paymentOptions.ip,
                body,
                out_trade_no,
                total_fee);
        }

        /*
         * 主动查询订单、关闭订单、申请退款.....等接口将陆续添加
         * https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_2
         */
    }
}
