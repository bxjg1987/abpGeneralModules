using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 参考官方文档：https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_1&index=1
    /// 微信返回的信息，没必要允许修改，所以全是只读的
    /// </summary>
    public class WeChatPaymentUnifyOrderResult : IWeChatMiniProgramPaymentNeedSign
    {
        #region MyRegion
        /// <summary>
        /// 返回状态码
        /// SUCCESS/FAIL 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public return_code return_code { get; private set; }
        /// <summary>
        /// 返回信息
        /// 返回信息，如非空，为错误原因    签名失败 参数格式校验错误
        /// </summary>
        public string return_msg { get; private set; }
        /// <summary>
        /// 调用接口提交的小程序ID
        /// </summary>
        public string appid { get; private set; }
        /// <summary>
        /// 调用接口提交的商户号
        /// </summary>
        public string mch_id { get; private set; }
        /// <summary>
        /// 自定义参数，可以为请求支付的终端设备号等
        /// </summary>
        public string device_info { get; private set; }
        /// <summary>
        /// 微信返回的随机字符串
        /// </summary>
        public string nonce_str { get; private set; }
        /// <summary>
        /// 微信返回的签名值
        /// https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_3
        /// </summary>
        public string sign { get; private set; }
        /// <summary>
        /// 业务结果 SUCCESS/FAIL
        /// </summary>
        public result_code result_code { get; private set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public err_code err_code { get; private set; }
        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string err_code_des { get; private set; }
        /// <summary>
        /// 交易类型，取值为：JSAPI，NATIVE，APP等
        /// https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_2
        /// </summary>
        public trade_type trade_type { get; private set; }
        /// <summary>
        /// 微信生成的预支付会话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string prepay_id { get; private set; }
        /// <summary>
        /// rade_type=NATIVE时有返回，此url用于生成支付二维码，然后提供给用户进行扫码支付。
        /// 注意：code_url的值并非固定，使用时按照URL格式转成二维码即可
        /// </summary>
        public string code_url { get; private set; }

        WeChatPaymentSecuret securet;

        //构造函数无法异步，所以从Strem初始化实例就不放构造函数了
        private WeChatPaymentUnifyOrderResult(WeChatPaymentSecuret securet)
        {
            this.securet = securet;
        }
        #endregion


        #region 组织数据返回给小程序端
        /// <summary>
        /// 创建一个微信小程序端需要的数据
        /// 由于签名会反射所有属性，因此只能定义为方法
        /// https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=7_7&index=3
        /// </summary>
        /// <returns></returns>
        public WeChatPaymentUnifyOrderResultForMiniProgram CreateMiniProgramResult()
        {
            return new WeChatPaymentUnifyOrderResultForMiniProgram(securet, appid, prepay_id);
        }
        #endregion

        //试试内部类呢？因为对于调用方来说 屏蔽WeChatPaymentSecuret是一个不错的选择
        /*public static async Task<WeChatPaymentUnifyOrderResult> LoadAsync(Stream stream, WeChatPaymentSecuret securet, CancellationToken cancellation = default)
        {
            var xd = await XDocument.LoadAsync(stream, LoadOptions.None, cancellation);
            var c = xd.Root;

            var p = new WeChatPaymentUnifyOrderResult();

            p.return_code = (string)c.Element("return_code");
            p.return_msg = (string)c.Element("return_msg");
            if (p.return_code != "SUCCESS")
                return p;

            p.appid = (string)c.Element("appid");
            p.mch_id = (string)c.Element("mch_id");
            p.device_info = (string)c.Element("device_info");
            p.nonce_str = (string)c.Element("nonce_str");
            p.sign = (string)c.Element("sign");
            p.result_code = (string)c.Element("result_code");//最好搞成枚举
            p.err_code = (string)c.Element("err_code");
            p.err_code_des = (string)c.Element("err_code_des");
            if (p.result_code != "SUCCESS")
                return p;

            p.trade_type = (string)c.Element("trade_type");
            p.prepay_id = (string)c.Element("prepay_id");
            p.code_url = (string)c.Element("code_url");



            return p;
        }
        */

        /// <summary>
        /// 统一下单，请求微信后会返回一个结果，此类用来根据响应流构建响应消息对象
        /// 内部会处理sign校验
        /// </summary>
        public class WeChatPaymentUnifyOrderResultFactory
        {
            WeChatPaymentSecuret securet;
            public WeChatPaymentUnifyOrderResultFactory(WeChatPaymentSecuret securet)
            {
                this.securet = securet;
            }
            /// <summary>
            /// 统一下单，请求微信后会返回一个结果，此类用来根据响应流构建响应消息对象
            /// 内部会处理sign校验
            /// </summary>
            /// <param name="stream"></param>
            /// <param name="cancellation"></param>
            /// <returns></returns>
            public async Task<WeChatPaymentUnifyOrderResult> LoadAsync(Stream stream, CancellationToken cancellation = default)
            {
                var xd = await XDocument.LoadAsync(stream, LoadOptions.None, cancellation);
                var c = xd.Root;

                var p = new WeChatPaymentUnifyOrderResult(this.securet);

                p.return_code = Enum.Parse<return_code>((string)c.Element("return_code"));
                p.return_msg = (string)c.Element("return_msg");
                if (p.return_code != return_code.SUCCESS)
                    return p;

                p.appid = (string)c.Element("appid");
                p.mch_id = (string)c.Element("mch_id");
                p.device_info = (string)c.Element("device_info");
                p.nonce_str = (string)c.Element("nonce_str");
                p.sign = (string)c.Element("sign");
                p.result_code = Enum.Parse<result_code>((string)c.Element("result_code"));
                p.err_code = Enum.Parse<err_code>((string)c.Element("err_code"));
                p.err_code_des = (string)c.Element("err_code_des");
                if (p.result_code != result_code.SUCCESS)
                    return p;

                p.trade_type = Enum.Parse<trade_type>((string)c.Element("trade_type"));
                p.prepay_id = (string)c.Element("prepay_id");
                p.code_url = (string)c.Element("code_url");


                if (!securet.CheckSign(p))
                    throw new InvalidCastException("微信小程序支付 > 统一下单 > 返回结果 > sign校验失败！");

                return p;
            }
        }
    }

    /// <summary>
    /// 当调用微信小程序支付的统一下单接口后，微信会返回一个数据，
    /// 按文档要求，我们的服务器应该将返回的数据组织一下，然后进行再次签名 并将组织的数据和再次签名的值返回给小程序端
    /// 参考 https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=7_7&index=3
    /// 此类型就 = 小程序端支付需要的数据 = 组织的数据 + 再次签名
    /// </summary>
    public class WeChatPaymentUnifyOrderResultForMiniProgram : IWeChatMiniProgramPaymentNeedSign
    {
        /// <summary>
        /// 小程序appid
        /// </summary>
        public string appId { get; private set; }
        /// <summary>
        /// 时间戳
        /// 时间戳从1970年1月1日00:00:00至今的秒数,即当前的时间
        /// https://www.jianshu.com/p/ea164c6ee987
        /// </summary>
        public string timeStamp { get; private set; } = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        public string nonceStr { get; private set; } = BXJG.Utils.RandomHelper.RandomBase64(12);
        /// <summary>
        /// 获取微信小程序支付 统一下单接口返回的订单号
        /// "prepay_id=xxxxxxx"
        /// </summary>
        public string package { get; private set; }
        /// <summary>
        /// 获取签名算法，目前规定死MD5
        /// </summary>
        public string signType { get; private set; } = Payment.sign_type.MD5.ToString();//目前定死
        /// <summary>
        /// 获取签名
        /// 注意每次调用此属性都会重新计算签名
        /// </summary>
        public string sign
        {
            get
            {
                return securet.GetSign(this);
            }
        }

        WeChatPaymentSecuret securet;

        public WeChatPaymentUnifyOrderResultForMiniProgram(
            WeChatPaymentSecuret securet,
            string appid,
            string prepay_id)
        {
            this.securet = securet;
            this.appId = appid;
            this.package = "prepay_id=" + prepay_id;
        }
    }
}