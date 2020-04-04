using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 前端支付完成后，微信回调我方接口时传递过来的数据结构
    /// 参考微信支付文档：https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_7
    /// </summary>
    public class WeChatPaymentNoticeResult : IWeChatMiniProgramPaymentNeedSign
    {
        #region MyRegion
        /// <summary>
        /// SUCCESS/FAIL 必填
        /// 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public return_code return_code { get; private set; }
        /// <summary>
        /// 返回信息 非必填
        /// 如非空，为错误原因 签名失败   参数格式校验错误
        /// </summary>
        public string return_msg { get; private set; }
        /// <summary>
        /// 小程序ID 必填
        /// </summary>
        public string appid { get; private set; }
        /// <summary>
        /// 商户号 必填
        /// </summary>
        public string mch_id { get; private set; }
        /// <summary>
        /// 设备号 非必填
        /// </summary>
        public string device_info { get; private set; }
        /// <summary>
        /// 随机字符串，不长于32位  必填
        /// </summary>
        public string nonce_str { get; private set; }
        /// <summary>
        /// 签名 必填
        /// https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_3
        /// </summary>
        public string sign { get; private set; }
        /// <summary>
        /// 签名类型 非必填
        /// 签名类型，目前支持HMAC-SHA256和MD5，默认为MD5
        /// </summary>
        public sign_type? sign_type { get; private set; } = Payment.sign_type.MD5;
        /// <summary>
        /// 业务结果 必填
        /// SUCCESS/FAIL
        /// </summary>
        public result_code result_code { get; private set; }
        /// <summary>
        /// 错误代码 非必填
        /// 官方没有写明确，暂时就字符串吧
        /// </summary>
        public string err_code { get; private set; }
        /// <summary>
        /// 错误代码描述 非必填
        /// </summary>
        public string err_code_des { get; private set; }
        /// <summary>
        /// 用户标识 必填
        /// </summary>
        public string openid { get; private set; }
        /// <summary>
        /// 是否关注公众账号  必填
        /// 用户是否关注公众账号，Y-关注，N-未关注
        /// </summary>
        public bool is_subscribe { get; private set; }
        /// <summary>
        /// 交易类型
        /// JSAPI、NATIVE、APP
        /// </summary>
        public trade_type trade_type { get; private set; }
        /// <summary>
        /// 付款银行 必填
        /// 太多了，直接用string类型，以后可能随时变动
        /// 银行类型，采用字符串类型的银行标识，银行类型见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public string bank_type { get; private set; }
        /// <summary>
        /// 订单金额 必填
        /// 订单总金额，单位为分
        /// </summary>
        public decimal total_fee { get; private set; }
        /// <summary>
        /// 应结订单金额 非必填
        /// 应结订单金额=订单金额-非充值代金券金额，应结订单金额<=订单金额。
        /// </summary>
        public decimal? settlement_total_fee { get; private set; }
        /// <summary>
        /// 货币种类  非必填
        /// 货币类型，符合ISO4217标准的三位字母代码，默认人民币：CNY，其他值列表详见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public fee_type? fee_type { get; private set; } = Payment.fee_type.CNY;
        /// <summary>
        /// 现金支付金额 必填
        /// 现金支付金额订单现金支付金额，详见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public decimal cash_fee { get; private set; }
        /// <summary>
        /// 现金支付货币类型 非必填
        /// 货币类型，符合ISO4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public fee_type? cash_fee_type { get; private set; } = Payment.fee_type.CNY;
        /// <summary>
        /// 总代金券金额 非必填
        /// 代金券金额<=订单金额，订单金额-代金券金额=现金支付金额，详见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public decimal? coupon_fee { get; private set; }
        /// <summary>
        /// 代金券使用数量 非必填
        /// </summary>
        public int? coupon_count { get; private set; }
        /// <summary>
        /// 第1张代金券类型 非必填
        /// </summary>
        public coupon_type? @coupon_type_0 { get; private set; }
        /// <summary>
        /// 第2张代金券类型 非必填
        /// </summary>
        public coupon_type? @coupon_type_1 { get; private set; }
        /// <summary>
        /// 第3张代金券类型 非必填
        /// </summary>
        public coupon_type? @coupon_type_2 { get; private set; }
        /// <summary>
        /// 第4张代金券类型 非必填
        /// </summary>
        public coupon_type? @coupon_type_3 { get; private set; }
        /// <summary>
        /// 第5张代金券类型 非必填
        /// </summary>
        public coupon_type? @coupon_type_4 { get; private set; }
        /// <summary>
        /// 第1个代金券ID 非必填 
        /// 代金券ID,$n为下标，从0开始编号
        /// 注意：只有下单时订单使用了优惠，回调通知才会返回券信息。
        /// 下列情况可能导致订单不可以享受优惠 https://pay.weixin.qq.com/wiki/doc/api/danpin.php?chapter=9_202&index=7#menu4
        /// </summary>
        public string coupon_id_0 { get; private set; }
        /// <summary>
        /// 第2个代金券ID 非必填 
        /// </summary>
        public string coupon_id_1 { get; private set; }
        /// <summary>
        /// 第3个代金券ID 非必填 
        /// </summary>
        public string coupon_id_2 { get; private set; }
        /// <summary>
        /// 第4个代金券ID 非必填 
        /// </summary>
        public string coupon_id_3 { get; private set; }
        /// <summary>
        /// 第5个代金券ID 非必填 
        /// </summary>
        public string coupon_id_4 { get; private set; }
        /// <summary>
        /// 第1个代金券支付金额 非必填
        /// </summary>
        public decimal? coupon_fee_0 { get; private set; }
        /// <summary>
        /// 第2个代金券支付金额 非必填
        /// </summary>
        public decimal? coupon_fee_1 { get; private set; }
        /// <summary>
        /// 第3个代金券支付金额 非必填
        /// </summary>
        public decimal? coupon_fee_2 { get; private set; }
        /// <summary>
        /// 第4个代金券支付金额 非必填
        /// </summary>
        public decimal? coupon_fee_3 { get; private set; }
        /// <summary>
        /// 第5个代金券支付金额 非必填
        /// </summary>
        public decimal? coupon_fee_4 { get; private set; }
        /// <summary>
        /// 微信支付订单号 必填
        /// </summary>
        public string transaction_id { get; private set; }
        /// <summary>
        /// 商户订单号 必填
        /// 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-|*@ ，且在同一个商户号下唯一
        /// </summary>
        public string out_trade_no { get; private set; }
        /// <summary>
        /// 商家数据包 非必填
        /// 原样返回
        /// </summary>
        public string attach { get; private set; }
        /// <summary>
        /// 支付完成时间 必填
        /// 支付完成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。
        /// 其他详见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public DateTimeOffset time_end { get; private set; }

        private WeChatPaymentNoticeResult() { }


        #endregion

        public class WeChatPaymentNoticeResultFactory
        {
            WeChatPaymentSecuret securet;
            public WeChatPaymentNoticeResultFactory(WeChatPaymentSecuret securet)
            {
                this.securet = securet;
            }
            /// <summary>
            /// 微信小程序支付结果通知提交过来的Stream 可以通过此方法解析
            /// </summary>
            /// <param name="stream"></param>
            /// <param name="cancellation"></param>
            /// <returns></returns>
            public async Task<WeChatPaymentNoticeResult> LoadAsync(Stream stream, CancellationToken cancellation = default)
            {
                var xd = await XDocument.LoadAsync(stream, LoadOptions.None, cancellation);
                var c = xd.Root;

                var p = new WeChatPaymentNoticeResult();

                p.return_code = Enum.Parse<return_code>((string)c.Element("return_code"));
                p.return_msg = (string)c.Element("return_msg");
                if (p.return_code != return_code.SUCCESS)
                    return p;

                p.appid = (string)c.Element("appid");
                p.mch_id = (string)c.Element("mch_id");
                p.device_info = (string)c.Element("device_info");
                p.nonce_str = (string)c.Element("nonce_str");
                p.sign = (string)c.Element("sign");

                var sign_type = (string)c.Element("sign_type");
                if (!string.IsNullOrWhiteSpace(sign_type))
                    p.sign_type = Enum.Parse<sign_type>(sign_type);

                p.result_code = Enum.Parse<result_code>((string)c.Element("result_code"));
                p.err_code = (string)c.Element("err_code");
                p.err_code_des = (string)c.Element("err_code_des");
                if (p.result_code != result_code.SUCCESS)
                    return p;

                p.openid = (string)c.Element("openid");
                p.is_subscribe = (bool)c.Element("is_subscribe");
                p.trade_type = Enum.Parse<trade_type>((string)c.Element("trade_type"));
                p.bank_type = (string)c.Element("bank_type");
                p.total_fee = (int)c.Element("total_fee") / 100m;
                var settlement_total_fee = (string)c.Element("settlement_total_fee");
                if (!string.IsNullOrWhiteSpace(settlement_total_fee))
                    p.settlement_total_fee = Convert.ToDecimal(settlement_total_fee) / 100;
                var fee_type = (string)c.Element("fee_type");
                if (!string.IsNullOrWhiteSpace(fee_type))
                    p.fee_type = Enum.Parse<fee_type>((string)c.Element("fee_type"));

                #region 现金
                var cash_fee = (string)c.Element("cash_fee");
                if (!string.IsNullOrWhiteSpace(cash_fee))
                    p.cash_fee = Convert.ToDecimal(cash_fee) / 100;

                var cash_fee_type = (string)c.Element("cash_fee_type");
                if (!string.IsNullOrWhiteSpace(cash_fee_type))
                    p.cash_fee_type = Enum.Parse<fee_type>((string)c.Element("cash_fee_type"));
                #endregion

                #region 代金券

                var coupon_fee = (string)c.Element("coupon_fee");
                if (!string.IsNullOrWhiteSpace(coupon_fee))
                    p.coupon_fee = Convert.ToDecimal(coupon_fee) / 100;


                var coupon_count = (string)c.Element("coupon_count");
                if (!string.IsNullOrWhiteSpace(coupon_count))
                    p.coupon_count = Convert.ToInt32(coupon_count);

                var coupon_type_0 = (string)c.Element("coupon_type_0");
                if (!string.IsNullOrWhiteSpace(coupon_type_0))
                {
                    p.coupon_type_0 = Enum.Parse<coupon_type>((string)c.Element("coupon_type_0"));
                    p.coupon_fee_0 = Convert.ToDecimal(c.Element("coupon_fee_0")) / 100;
                }
                var coupon_type_1 = (string)c.Element("coupon_type_1");
                if (!string.IsNullOrWhiteSpace(coupon_type_1))
                {
                    p.coupon_type_1 = Enum.Parse<coupon_type>((string)c.Element("coupon_type_1"));
                    p.coupon_fee_1 = Convert.ToDecimal(c.Element("coupon_fee_1")) / 100;
                }
                var coupon_type_2 = (string)c.Element("coupon_type_2");
                if (!string.IsNullOrWhiteSpace(coupon_type_2))
                {
                    p.coupon_type_2 = Enum.Parse<coupon_type>((string)c.Element("coupon_type_2"));
                    p.coupon_fee_2 = Convert.ToDecimal(c.Element("coupon_fee_2")) / 100;
                }
                var coupon_type_3 = (string)c.Element("coupon_type_3");
                if (!string.IsNullOrWhiteSpace(coupon_type_3))
                {
                    p.coupon_type_3 = Enum.Parse<coupon_type>((string)c.Element("coupon_type_3"));
                    p.coupon_fee_3 = Convert.ToDecimal(c.Element("coupon_fee_3")) / 100;
                }
                var coupon_type_4 = (string)c.Element("coupon_type_4");
                if (!string.IsNullOrWhiteSpace(coupon_type_4))
                {
                    p.coupon_type_4 = Enum.Parse<coupon_type>((string)c.Element("coupon_type_4"));
                    p.coupon_fee_4 = Convert.ToDecimal(c.Element("coupon_fee_4")) / 100;
                }
                p.coupon_id_0 = (string)c.Element("coupon_id_0");
                p.coupon_id_1 = (string)c.Element("coupon_id_1");
                p.coupon_id_2 = (string)c.Element("coupon_id_2");
                p.coupon_id_3 = (string)c.Element("coupon_id_3");
                p.coupon_id_4 = (string)c.Element("coupon_id_4");
                #endregion

                p.transaction_id = (string)c.Element("transaction_id");
                p.out_trade_no = (string)c.Element("out_trade_no");
                p.attach = (string)c.Element("attach");
                p.time_end = DateTimeOffset.Parse((string)c.Element("time_end"));
               
                if (!securet.CheckSign(p))
                    throw new InvalidCastException("微信小程序支付 > 支付结果通知 > 微信提交过来的数据 > sign校验失败！");

                return p;
            }
        }
    }
}
