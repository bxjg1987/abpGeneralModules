using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BXJG.WeChat.MiniProgram
{
    /// <summary>
    /// 通过code获取到的微信用户信息 + 前端提交的原始数据
    /// </summary>
    public class WeChatUser
    {
        /// <summary>
        /// 用户提交上来的原始json数据
        /// </summary>
        public JsonElement Input { get; set; }
        public string session_key { get; set; }
        public string openid { get; set; }
        public string unionid { get; set; }
    }
}
