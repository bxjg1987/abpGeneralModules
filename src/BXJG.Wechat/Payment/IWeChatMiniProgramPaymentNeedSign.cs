using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.WeChat.Payment
{
    /// <summary>
    /// 辅助实现微信小程序支付中的sign校验的接口
    /// </summary>
    public interface IWeChatMiniProgramPaymentNeedSign
    {
        string sign { get;  }
    }
}
