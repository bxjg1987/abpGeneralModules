using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using AutoMapper;
namespace BXJG.File
{
    /// <summary>
    /// 获取文件时的视图模型
    /// </summary>
    [AutoMapFrom(typeof(BXJGFileEntty))]
    public class BXJGFileDto
    {
        /// <summary>
        /// 文件唯一Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 助记码
        /// </summary>
        public string MnemonicCode { get; set; }
        /// <summary>
        /// 文件的md5值
        /// </summary>
        public string MD5 { get; set; }
        ///// <summary>
        ///// 本地相对路径
        ///// </summary>
        //public string Path { get; set; }
        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelativePath { get; set; }
        /// <summary>
        /// 文件大小kb
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// MIME类型字符串
        /// </summary>
        public string Mime { get; set; }
    }
}
