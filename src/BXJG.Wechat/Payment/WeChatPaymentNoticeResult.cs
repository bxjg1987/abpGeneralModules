using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 前端支付完成后，微信回调我方接口时传递过来的数据结构
    /// 参考微信支付文档：https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_7
    /// </summary>
    [XmlRoot("xml")]
    public class WeChatPaymentNoticeResult
    {
        /// <summary>
        /// SUCCESS/FAIL
        /// 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息，如非空，为错误原因 签名失败   参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }
        /// <summary>
        /// 小程序ID
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 设备号
        /// </summary>
        public string device_info { get; set; }
        /// <summary>
        /// 随机字符串，不长于32位
        /// </summary>
        public string nonce_str { get; set; }
        /// <summary>
        /// 签名 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_3
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 签名类型
        /// 签名类型，目前支持HMAC-SHA256和MD5，默认为MD5
        /// </summary>
        public string sign_type { get; set; }
        /// <summary>
        /// 业务结果 SUCCESS/FAIL
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 用户标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 是否关注公众账号
        /// 用户是否关注公众账号，Y-关注，N-未关注
        /// </summary>
        public string is_subscribe { get; set; }
        /// <summary>
        /// 交易类型
        /// JSAPI、NATIVE、APP
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 付款银行
        /// 银行类型，采用字符串类型的银行标识，银行类型见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public string bank_type { get; set; }
        /// <summary>
        /// 订单金额
        /// 订单总金额，单位为分
        /// </summary>
        public int total_fee { get; set; }
        /// <summary>
        /// 应结订单金额
        /// 应结订单金额=订单金额-非充值代金券金额，应结订单金额<=订单金额。
        /// </summary>
        public int settlement_total_fee { get; set; }
        /// <summary>
        /// 货币种类
        /// 货币类型，符合ISO4217标准的三位字母代码，默认人民币：CNY，其他值列表详见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public string fee_type { get; set; }
        /// <summary>
        /// 现金支付金额
        /// 现金支付金额订单现金支付金额，详见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public int cash_fee { get; set; }
        /// <summary>
        /// 现金支付货币类型
        /// 货币类型，符合ISO4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public string cash_fee_type { get; set; }
        /// <summary>
        /// 总代金券金额
        /// 代金券金额<=订单金额，订单金额-代金券金额=现金支付金额，详见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public int coupon_fee { get; set; }
        /// <summary>
        /// 代金券使用数量
        /// </summary>
        public int coupon_count { get; set; }
        /// <summary>
        /// 代金券类型
        /// CASH--充值代金券
        /// NO_CASH---非充值代金券
        /// 并且订单使用了免充值券后有返回（取值：CASH、NO_CASH）。$n为下标,从0开始编号，举例：coupon_type_0
        /// 注意：只有下单时订单使用了优惠，回调通知才会返回券信息。
        /// 下列情况可能导致订单不可以享受优惠： https://pay.weixin.qq.com/wiki/doc/api/danpin.php?chapter=9_202&index=7#menu4
        /// </summary>
        public string @coupon_type_0 { get; set; }
        public string @coupon_type_1 { get; set; }
        public string @coupon_type_2 { get; set; }
        public string @coupon_type_3 { get; set; }
        public string @coupon_type_4 { get; set; }
        /// <summary>
        /// 代金券ID
        /// 代金券ID,$n为下标，从0开始编号
        /// 注意：只有下单时订单使用了优惠，回调通知才会返回券信息。
        /// 下列情况可能导致订单不可以享受优惠 https://pay.weixin.qq.com/wiki/doc/api/danpin.php?chapter=9_202&index=7#menu4
        /// </summary>
        public string coupon_id_0 { get; set; }
        public string coupon_id_1 { get; set; }
        public string coupon_id_2 { get; set; }
        public string coupon_id_3 { get; set; }
        public string coupon_id_4 { get; set; }
        /// <summary>
        /// 单个代金券支付金额
        /// 单个代金券支付金额,$n为下标，从0开始编号
        /// </summary>
        public int coupon_fee_0 { get; set; }
        public int coupon_fee_1 { get; set; }
        public int coupon_fee_2 { get; set; }
        public int coupon_fee_3 { get; set; }
        public int coupon_fee_4 { get; set; }
        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 商户订单号
        /// 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-|*@ ，且在同一个商户号下唯一
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 商家数据包，原样返回
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 支付完成时间
        /// 支付完成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。
        /// 其他详见 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public string time_end { get; set; }
    }
}
