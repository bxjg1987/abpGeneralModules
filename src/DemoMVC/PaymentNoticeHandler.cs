using BXJG.WeChat.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoMVC
{
    public class PaymentNoticeHandler : BXJG.WeChat.Payment.IWeChatPaymentNoticeHandler
    {
        public Task PaymentNoticeAsync(WeChatPaymentNoticeContext context)
        {
            //做你的业务处理
            //若你业务处理没成功可以抛出异常，微信后续会继续尝试推送过来
            //你需要考虑并发的问题
            return Task.CompletedTask;
        }
    }
}
