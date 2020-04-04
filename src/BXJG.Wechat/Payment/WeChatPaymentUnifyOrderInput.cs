using BXJG.Utils;
using BXJG.WeChat.MiniProgram;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 统一下单，要提交给微信的数据模型；数据结构与类型完全与官方保持一致，不做任何转换
    /// https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_1&index=1
    /// </summary>
    public class WeChatPaymentUnifyOrderInput: IWeChatMiniProgramPaymentNeedSign// IPostToWeChat
    {
        /// <summary>
        /// 获取appid
        /// </summary>
        public string appid { get; private set; }
        /// <summary>
        /// 获取商户号
        /// </summary>
        public string mch_id { get; private set; }
        /// <summary>
        /// 设备号 可选
        /// 自定义参数，可以为终端设备号(门店号或收银设备ID)，PC网页或公众号内支付可以传"WEB"
        /// </summary>
        public string device_info { get; set; }
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        public string nonce_str { get; private set; }
        /// <summary>
        /// 获取签名 你不要管这个
        /// </summary>
        public string sign
        {
            get
            {
                return securet.GetSign(this);
            }
        }
        /// <summary>
        /// 获取签名类型
        /// 目前写死的md5，因为微信要求此参数可选，所以用了符号：?
        /// </summary>
        public sign_type? sign_type { get; private set; } = Payment.sign_type.MD5;//目前写死 只能md5
        /// <summary>
        /// 商品描述 必填
        /// 商品简单描述，该字段请按照规范传递，具体请见参数规定
        /// https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 商品详情 可选
        /// 商品详细描述，对于使用单品优惠的商户，该字段必须按照规范上传，
        /// https://pay.weixin.qq.com/wiki/doc/api/danpin.php?chapter=9_102&index=2
        /// </summary>
        public string detail { get; set; }
        /// <summary>
        /// 附加数据 可选
        /// 附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用。
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 商户订单号 必填
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 标价币种 固定CNY
        /// </summary>
        public fee_type? fee_type { get; private set; } = Payment.fee_type.CNY;
        /// <summary>
        /// 标价金额  必填
        /// 单位元
        /// </summary>
        public decimal total_fee { get; set; }
        /// <summary>
        /// 终端IP
        /// 内部目前从配置中获取ip
        /// </summary>
        public string spbill_create_ip { get; private set; }
        /// <summary>
        /// 交易起始时间 可选
        /// 默认当前时间
        /// </summary>
        public DateTime? time_start { get; set; } = DateTime.Now;
        /// <summary>
        /// 交易结束时间 可选
        /// 默认5分钟
        /// </summary>
        public DateTime? time_expire { get; set; } = DateTime.Now.AddMinutes(5);
        /// <summary>
        /// 订单优惠标记 可选
        /// 订单优惠标记，使用代金券或立减优惠功能时需要的参数，
        /// https://pay.weixin.qq.com/wiki/doc/api/tools/sp_coupon.php?chapter=12_1
        /// </summary>
        public string goods_tag { get; set; }
        /// <summary>
        /// 异步接收微信支付结果通知的回调地址，通知url必须为外网可访问的url，不能携带参数。
        /// 内部会从选项对象中获取
        /// </summary>
        public string notify_url { get; private set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public trade_type trade_type { get; private set; } = trade_type.JSAPI;//反正固定的 就不搞枚举了
        /// <summary>
        /// 商品ID 可选
        /// trade_type=NATIVE时，此参数必传。此参数为二维码中包含的商品ID，商户自行定义。
        /// </summary>
        public string product_id { get; set; }
        /// <summary>
        /// 指定支付方式 可选
        /// 上传此参数no_credit--可限制用户不能使用信用卡支付
        /// </summary>
        public string limit_pay { get; set; }
        /// <summary>
        /// 不解释 注意是可选的
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 电子发票入口开放标识 可选
        /// Y，传入Y时，支付成功消息和支付详情页将出现开票入口。需要在微信支付商户平台或微信公众平台开通电子发票功能，传此字段才可生效
        /// </summary>
        public string receipt { get; set; }
        /// <summary>
        /// 场景信息 可选 
        /// 该字段常用于线下活动时的场景信息上报，支持上报实际门店信息，商户也可以按需求自己上报相关信息。该字段为JSON对象数据，对象格式为{"store_info":{"id": "门店ID","name": "名称","area_code": "编码","address": "地址" }} ，字段详细说明请点击行前的+展开
        /// </summary>
        public string scene_info { get; set; }


        WeChatPaymentSecuret securet;

        public WeChatPaymentUnifyOrderInput(
            WeChatPaymentSecuret securet,
            string appid,
            string mch_id,
            string notify_url,
            string ip,
            string body,
            string out_trade_no,
            decimal total_fee)
        {
            this.securet = securet;

            this.appid = appid;
            this.mch_id = mch_id;
            ResetNonce();
            this.body = body;
            this.out_trade_no = out_trade_no;
            this.total_fee = total_fee;
            this.spbill_create_ip = ip;//先这么来，以后可能想个法获取服务器的外网ip
            this.notify_url = notify_url;
        }

        /// <summary>
        /// 重置随机字符串
        /// </summary>
        public string ResetNonce(int k = 12)
        {
            nonce_str = RandomHelper.RandomBase64(k);
            return nonce_str;
        }
        ///// <summary>
        ///// 按微信规定的方式计算签名
        ///// </summary>
        ///// <returns></returns>
        //public string ComputationalSignature()
        //{
        //    return securet.GetSign(this);

        //}
        /// <summary>
        /// 设置单品优惠
        /// 目前木有实现
        /// </summary>
        public void SetDetail()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 将消息转换为微信要求的xml格式
        /// 这种转换因为有微信规定的规则，所以此功能定义在实例里
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            var x = this;
            //也许某些参数没有值的时候可以不传，没有测试过，如果可行 可以减少传输的数据量
            return $@"; <xml>
                            <appid>{x.appid}</appid>
                            <mch_id>{x.mch_id}</mch_id>
                            <device_info>{x.device_info}</device_info>
                            <nonce_str>{x.nonce_str}</nonce_str>
                            <sign>{x.sign}</sign>
                            <sign_type>{x.sign_type}</sign>
                            <body>{x.body}</body>
                            <detail><![CDATA[{x.detail}]]></detail>
                            <attach>{x.attach}</attach>
                            <out_trade_no>{x.out_trade_no}</out_trade_no>
                            <fee_type>{x.fee_type}</fee_type>
                            <total_fee>{ Convert.ToInt32(x.total_fee * 100)}</total_fee>
                            <spbill_create_ip>{x.spbill_create_ip}</spbill_create_ip>
                            <time_start>{x.time_start?.ToString("yyyyMMddHHmmss")}</time_start>
                            <time_expire>{x.time_expire?.ToString("yyyyMMddHHmmss")}</time_expire>
                            <goods_tag>{x.goods_tag}</goods_tag>
                            <notify_url>{x.notify_url}</notify_url>
                            <trade_type>{x.trade_type}</trade_type>
                            <product_id>{x.product_id}</product_id>
                            <limit_pay>{x.limit_pay}</limit_pay>
                            <openid>{x.openid}</openid>
                            <receipt>{x.receipt}</receipt>
                            <scene_info>{x.scene_info}</scene_info>
                        </xml>";
        }
    }
}
