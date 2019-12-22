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
               BXJGFileConsts.FileUploadSettingGroupLocalizableString.BXJGL());

            return new[] {
                new SettingDefinition(
                    BXJGFileConsts.FileUploadExtensionSetting,
                    BXJGFileConsts.FileUploadExtensionDefaultSetting,
                    BXJGFileConsts.FileUploadExtensionSettingDisplayNameLocalizableString.BXJGL(),
                    group,
                    BXJGFileConsts.FileUploadExtensionSettingDescriptionLocalizableString.BXJGL(),
                    isVisibleToClients:true)
            };
        }
    }
}