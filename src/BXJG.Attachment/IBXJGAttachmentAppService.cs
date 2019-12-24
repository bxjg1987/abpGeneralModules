using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using BXJG.File;

namespace BXJG.Attachment
{
    /// <summary>
    /// 附件管理应用服务接口
    /// <para>当业务表单数据与附件分开上传时可以通过此接口单独处理附件。尤其是大文件通常分片上传时，文件与表单无法一起处理</para>
    /// <para>当业务表单业务数据与小文件一起提交时，应在业务模块对应的应用服务接口中去自己处理附件，不应使用此接口，因为应用服务接口是面向用例的</para>
    /// </summary>
    public interface IBXJGAttachmentAppService<
         TFileDto,
         TCheckUploadAttachmentInput,
         TCheckUploadResult,
         TDownloadAttachmentInput,
         TDownloadOutput> : IApplicationService
    {
        /// <summary>
        /// 当文件已完全上传后，调用此方法创建一个文件对象返回给客户端，客户端后续应该将实体和文件信息提交后，由BXJGAttachmentManager做文件和实体的关联形成附件
        /// </summary>
        /// <param name="tempFilePath">上传文件的临时存储路径</param>
        /// <param name="md5">上传文件时一并提供的md5值</param>
        /// <param name="fileTitle">文件名，如：a.txt</param>
        /// <param name="module">模块名</param>
        /// <param name="permission">权限名</param>
        /// <param name="mnemonicCode">文件的助记码</param>
        /// <param name="keywords">文件关联的关键字</param>
        /// <returns></returns>
        [RemoteService(false)]//不生成动态api
        Task<TFileDto> CreateAsync(string tempFilePath, string md5, string fileTitle, string module, string permission, string mnemonicCode = "", string keywords = "");
        /// <summary>
        /// 当文件已完全上传后，调用此方法创建一个文件对象返回给客户端，客户端后续应该将实体和文件信息提交后，由BXJGAttachmentManager做文件和实体的关联形成附件
        /// </summary>
        /// <param name="stream">上传文件关联的流</param>
        /// <param name="md5">上传文件时一并提供的md5值</param>
        /// <param name="fileTitle">文件名，如：a.txt</param>
        /// <param name="module">模块名</param>
        /// <param name="permission">权限名</param>
        /// <param name="mnemonicCode">文件的助记码</param>
        /// <param name="keywords">文件关联的关键字</param>
        /// <returns></returns>
        [RemoteService(false)]//不生成动态api
        Task<TFileDto> CreateAsync(Stream stream, string md5, string fileTitle, string module, string permission, string mnemonicCode = "", string keywords = "");
        /// <summary>
        /// 检查是否可以上传。判断权限、检查文件类型、判断是否已超过当前租户允许的总磁盘容量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TCheckUploadResult> CheckUploadAsync(TCheckUploadAttachmentInput input);
        /// <summary>
        /// 下载附件。此方法先验证下载权限，若通过则将返回附件关联的文件信息和对应的流（Stream）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RemoteService(false)]
        Task<TDownloadOutput> DownloadAsync(TDownloadAttachmentInput input);
    }

    public interface IBXJGAttachmentAppService : IBXJGAttachmentAppService<BXJGFileDto, BXJGCheckUploadAttachmentInput, BXJGCheckUploadResult<BXJGFileDto>, BXJGDownloadAttachmentInput, BXJGDownloadOutput>
    { }
}
