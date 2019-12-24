using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using Abp.Localization;

namespace BXJG.File
{
    public class BXJGFileSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            var group = new SettingDefinitionGroup(
               BXJGFileConsts.FileUploadSettingGroup,
               BXJGFileConsts.FileUploadSettingGroupLocalizableString.BXJGFileL());

            return new[] {
                new SettingDefinition(
                    BXJGFileConsts.FileUploadExtensionSetting,
                    BXJGFileConsts.FileUploadExtensionDefaultSetting,
                    BXJGFileConsts.FileUploadExtensionSettingDisplayNameLocalizableString.BXJGFileL(),
                    group,
                    BXJGFileConsts.FileUploadExtensionSettingDescriptionLocalizableString.BXJGFileL(),
                    isVisibleToClients:true)
            };
        }
    }
}