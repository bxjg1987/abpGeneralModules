using Abp.Application.Features;
using Abp.Localization;
using Abp.Runtime.Validation;
using Abp.UI.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.File
{
    public class BXJGFileFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            //租户上传的所有文件(包含附件之类的)总大小限制，单位Mb
            context.Create(
                BXJGFileConsts.MaxFileUploadSizeFeature,
                BXJGFileConsts.MaxFileUploadSizeFeatureDefault.ToString(),//2gb
                BXJGFileConsts.MaxFileUploadSizeFeatureDisplayNameLocalizableString.BXJGFileL(),
                BXJGFileConsts.MaxFileUploadSizeFeatureDiscriptionLocalizableString.BXJGFileL(),
                FeatureScopes.All,
                new SingleLineStringInputType(new NumericValueValidator(0, (int)BXJGFileConsts.MaxFileUploadSizeFeatureDefault))); 
        }
    }
}
