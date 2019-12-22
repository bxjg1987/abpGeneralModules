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
    public interface IAttachmentAppService<
         TFileDto,
         TCheckUploadAttachmentInput,
         TCheckUploadResult,
         TDownloadAttachmentInput,
         TDownloadOutput> : IApplicationService
    {
        /// <summary>
        /// 增加一个附件
        /// <para>前端不要直接调动此接口，应调用UI层提供的接口</para>
        /// </summary>
        /// <param name="tempFilePath"></param>
        /// <param name="md5">由调用方提供的文件md5，内部将与文件真实md5进行比对</param>
        /// <param name="fileTitle"></param>
        /// <param name="module"></param>
        /// <param name="permission"></param>
        /// <param name="mnemonicCode"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [RemoteService(false)]//不生成动态api
        Task<TFileDto> CreateAsync(string tempFilePath, string md5, string fileTitle, string module, string permission, string mnemonicCode = "", string keywords = "");
        /// <summary>
        /// 增加一个附件
        /// <para>前端不要直接调动此接口，应调用UI层提供的接口</para>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="md5">由调用方提供的文件md5，内部将与文件真实md5进行比对</param>
        /// <param name="fileTitle"></param>
        /// <param name="module"></param>
        /// <param name="permission"></param>
        /// <param name="mnemonicCode"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [RemoteService(false)]//不生成动态api
        Task<TFileDto> CreateAsync(Stream stream, string md5, string fileTitle, string module, string permission, string mnemonicCode = "", string keywords = "");
        /// <summary>
        /// 由于上传文件时是先保存文件，再判断权限、大小、文件类型等限制，由于保存文件是耗时的，若后续的验证不过，则客户体验不好,因此专门提供此接口先做判断
        /// <para>不要妄想传入非法参数绕过限制，因为正式上传接口CreateAsync内会再做一次检查</para>
        /// <para>若是上传小文件(包括分片上传)时，可以不做检查</para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TCheckUploadResult> CheckUploadAsync(TCheckUploadAttachmentInput input);
        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RemoteService(false)]
        Task<TDownloadOutput> DownloadAsync(TDownloadAttachmentInput input);
    }

    public interface IAttachmentAppService : IAttachmentAppService<BXJGFileDto, CheckUploadAttachmentInput, CheckUploadResult<BXJGFileDto>, DownloadAttachmentInput, DownloadOutput>
    { }
}
