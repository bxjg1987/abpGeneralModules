using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.File
{
    public class BXJGFileConsts
    {
        public const string LocalizationSourceName = "BXJGFile";


        public const int FileNameMaxLength = 200;
        public const int FileMnemonicCodeMaxLength = 200;
        public const int FileMD5MaxLength = 32;
        public const int FilePathMaxLength = 500;
        public const int FileKeywordsMaxLength = 2000;

        public const string UploadRoot = "Uploads";
        public const string UploadTemp = "Temp";
        public const string dir = "files";

        #region 特征
        public const string MaxFileUploadSizeFeature = "MaxUploadFileSize1";
        public const string MaxFileUploadSizeFeatureDisplayNameLocalizableString = "MaxFileUploadSizeDisplayName";
        public const string MaxFileUploadSizeFeatureDiscriptionLocalizableString = "MaxFileUploadSizeDiscription";
        public const long MaxFileUploadSizeFeatureDefault = 1024 * 2;

        #endregion

        #region 设置
        /// <summary>
        /// 文件上传设置组的键
        /// </summary>
        public const string FileUploadSettingGroup = "FileUploadSettingGroup1";
        /// <summary>
        /// 文件上传设置组的本地化字符串的键
        /// </summary>
        public const string FileUploadSettingGroupLocalizableString = "FileUploadSettingGroup";
        /// <summary>
        /// 允许上传的文件扩展名设置键
        /// </summary>
        public const string FileUploadExtensionSetting = "FileUploadExtension1";
        /// <summary>
        /// 允许上传的文件扩展名设置对应的标题名的本地化键
        /// </summary>
        public const string FileUploadExtensionSettingDisplayNameLocalizableString = "FileUploadExtensionSettingDisplayName";
        /// <summary>
        /// 允许上传的文件扩展名设置对应的描述信息的本地化键
        /// </summary>
        public const string FileUploadExtensionSettingDescriptionLocalizableString = "FileUploadExtensionSettingDescription";
        /// <summary>
        /// 默认允许上传的文件类型
        /// </summary>
        public const string FileUploadExtensionDefaultSetting = ".zip .psd .jpg .jpeg .png .bmp .gif .txt .pdf .doc .docx .xls .xlsx .ppt .pptx";
        
      
        #endregion
    }
}
