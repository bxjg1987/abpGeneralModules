using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BXJG.File
{
    public static class BXJGFileExtentions
    {
        /// <summary>
        /// 获取文件模块的配置对象
        /// </summary>
        /// <param name="moduleConfigurations"></param>
        /// <returns></returns>
        public static BXJGFileModuleConfig BXJGFielModuleConfig(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<BXJGFileModuleConfig>();
        }
        public static string GetMD5ByFilePath(this string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                return file.GetMD5();
            }
        }
        public static string GetMD5(this Stream stream)
        {
            byte[] retVal = null;

            using (var md5 = MD5.Create())
            {
                retVal = md5.ComputeHash(stream);
            }

            if (retVal == null || retVal.Length == 0)
                throw new Exception();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("X2"));
            }
            return sb.ToString();
        }


        //public static IEnumerable<SettingDefinition> DefineBXJGSettingAllowFileType(this SettingProvider appSettingProvider)
        //{
        //    var group = new SettingDefinitionGroup(
        //        BXJGConsts.FileUploadSettingGroup,
        //        L(BXJGConsts.FileUploadSettingGroupLocalizableString));

        //    return new[] {
        //        new SettingDefinition(
        //            BXJGConsts.FileUploadExtensionSetting,
        //            BXJGConsts.FileUploadExtensionDefaultSetting,
        //            L(BXJGConsts.FileUploadExtensionSettingDisplayNameLocalizableString),
        //            group,
        //            L(BXJGConsts.FileUploadExtensionSettingDescriptionLocalizableString),
        //            isVisibleToClients:true)
        //    };
        //}

        public static ILocalizableString BXJGFileL(this string name)
        {
            return new LocalizableString(name, BXJGFileConsts.LocalizationSourceName);
        }

    }
}
