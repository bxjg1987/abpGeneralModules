using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.WeChat.Payment
{
    public static class HttpContextExtensions
    {
        //public static Task ResponseWeChatAsync(this HttpContext context, string code, string msg = "")
        //{
        //    //将来可以考虑封装。
        //    //定义IWeChatResponseMessage 里面定义ToXML扩展方法，不同的消息有不同实现
        //    //可以进一步为HttpContext和HttpRequest提供扩展方法，简化调用
        //    //因为目前对微信整体开发了解不全面，不要盲目封装
        //    //var content = $@"<xml>
        //    //                    <return_code><![CDATA[{code}]]></return_code>
        //    //                    <return_msg><![CDATA[{msg}]]></return_msg>
        //    //                </xml>";
        //    //await context.Response.WriteAsync(content, context.RequestAborted);

        //    return context.Response.ResponseWeChatAsync(code, msg);
        //}
        public static async Task ResponseWeChatAsync(this HttpResponse response, string code, string msg = "")
        {
            //将来可以考虑封装。
            //定义IWeChatResponseMessage 里面定义ToXML扩展方法，不同的消息有不同实现
            //可以进一步为HttpContext和HttpRequest提供扩展方法，简化调用
            //因为目前对微信整体开发了解不全面，不要盲目封装
            var content = $@"<xml>
                                <return_code><![CDATA[{code}]]></return_code>
                                <return_msg><![CDATA[{msg}]]></return_msg>
                            </xml>";
            await response.WriteAsync(content, response.HttpContext.RequestAborted);
        }
        public static  Task ResponseWeChatSuccessAsync(this HttpResponse response)
        {
            return response.ResponseWeChatAsync(Consts.SUCCESS);
        }

        public static Task ResponseWeChatFailAsync(this HttpResponse response,string msg)
        {
            return response.ResponseWeChatAsync(Consts.SUCCESS);
        }
    }
}
