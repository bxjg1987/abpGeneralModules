using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BXJG.Attachment
{
    /// <summary>
    /// 提供附件模块的相关扩展方法
    /// </summary>
    public static class BXJGAttachmentExtentions
    {
        /// <summary>
        /// 获取附件模块的配置对象
        /// </summary>
        /// <param name="moduleConfigurations"></param>
        /// <returns></returns>
        public static BXJGAttachmentModuleConfig BXJGAttachmentModuleConfig(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<BXJGAttachmentModuleConfig>();
        }
        /// <summary>
        /// 获取附件模块的本地化对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILocalizableString BXJGAttachmentL(this string name)
        {
            return new LocalizableString(name, BXJGAttachmentConsts.LocalizationSourceName);
        }
    }
}
