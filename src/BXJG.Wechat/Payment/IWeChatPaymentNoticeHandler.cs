using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 当微信用户支付成功后，微信会回调我们的接口以通知我们：有用户支付成功了...
    /// 支付结果通知中间件(WeChatPaymentNoticeMiddleware)将拦截请求，然后解析微信提交过来的数据，然后调用此接口进行业务处理
    /// 您应该实现此接口来完成自己的业务处理，并在配置时指定你的类型（本质上是将你的类型注册到ioc容器）
    /// </summary>
    public interface IWeChatPaymentNoticeHandler
    {
        //目前微信规定的只返回成功或失败，简单的办法是返回Task，当无异常时认为成功，否则通过异常捕获失败原因
        //理想的方式 考虑微信将来接口的变动，我们应该始终定义一个返回值，即便将来微信接口要求更多的返回字段，我们接口的定义至少不用动 只需要修改返回的模型的定义
        //但是微信接口变动的情况可能性不大，暂时就现在这种方式吧
        Task PaymentNoticeAsync(WeChatPaymentNoticeContext context);
    }
}
