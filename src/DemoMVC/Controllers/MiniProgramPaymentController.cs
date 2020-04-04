using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BXJG.WeChat.Payment;
using DemoMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiniProgramPaymentController : ControllerBase
    {
        private readonly BXJG.WeChat.Payment.WeChatPaymentService weChatPaymentService;

        public MiniProgramPaymentController(WeChatPaymentService weChatPaymentService)
        {
            this.weChatPaymentService = weChatPaymentService;
        }

        public async Task<BXJG.WeChat.Payment.WeChatPaymentUnifyOrderResultForMiniProgram> PaymentAsync(PaymentInput intput)
        {
            //做一堆业务判断
            //金额 订单 优惠 ... 你的业务
            var order = weChatPaymentService.Create("商品描述", "本地订单号", 3.5m);
            order.product_id = "aaa";
            //继续配置微信需要的消息参数
            var rt = await weChatPaymentService.PayAsync(order, base.HttpContext.RequestAborted);
            //你的业务再判断
            return rt.CreateMiniProgramResult();
        }
    }
}