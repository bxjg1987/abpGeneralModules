using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Linq;
using BXJG.File;
using Abp.Dependency;

namespace BXJG.Attachment
{
    /// <summary>
    /// 附件管理应用服务类，主要提供附件上传、下载及其权限和规则验证
    /// <para>扩展点1：类中方法的输入和输出数据基本都使用泛型，便于扩展</para>
    /// <para>扩展点2：也可以继承此类进行扩展</para>
    /// <para>提供默认实现BXJGAttachmentAppService类</para>
    /// </summary>
    /// <typeparam name="TFileDto">上传前若存在、创建附件成功后 将返回的文件信息</typeparam>
    /// <typeparam name="TCheckUploadAttachmentInput">上传前进行权限和规则验证时的输入参数类型</typeparam>
    /// <typeparam name="TCheckUploadResult">上传前进行权限和规则验证时的返回值类型</typeparam>
    /// <typeparam name="TDownloadAttachmentInput">下载附件时提供的输入参数的类型</typeparam>
    /// <typeparam name="TDownloadOutput">下载附件时的返回值的类型</typeparam>
    /// <typeparam name="TFileEntity">文件实体类型</typeparam>
    /// <typeparam name="TAttachmentEntity">附件实体类型</typeparam>
    /// <typeparam name="TFileManager">文件管理领域服务的类型</typeparam>
    /// <typeparam name="TAttachmentManager">附件管理的领域服务类型</typeparam>
    public  class BXJGAttachmentAppService<
         TFileDto,
         TCheckUploadAttachmentInput,
         TCheckUploadResult,
         TDownloadAttachmentInput,
         TDownloadOutput,
         TFileEntity,
         TAttachmentEntity,
         TFileManager,
         TAttachmentManager> : ApplicationService ,
        IBXJGAttachmentAppService<TFileDto, TCheckUploadAttachmentInput, TCheckUploadResult, TDownloadAttachmentInput, TDownloadOutput>
        where TCheckUploadAttachmentInput : BXJGCheckUploadAttachmentInput
        where TCheckUploadResult : BXJGCheckUploadResult<TFileDto>, new()
        where TDownloadAttachmentInput : BXJGDownloadAttachmentInput
        where TDownloadOutput : BXJGDownloadOutput, new()
        where TFileEntity : BXJGFileEntty, new()
        where TAttachmentEntity : BXJGAttachmentEntity<TFileEntity>, new()
        where TFileManager : BXJGLocalFileManager<TFileEntity>
        where TAttachmentManager : BXJGAttachmentManager<TFileEntity, TAttachmentEntity, TFileManager>
    {
        private readonly IRepository<TAttachmentEntity, long> repository;
        private readonly IRepository<TFileEntity, long> fileRepository;
        private readonly TAttachmentManager attachmentManager;
        private readonly TFileManager fileManager;
        private readonly IBXJGAttachmentPermissionChecker attachmentPermissionChecker;
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public BXJGAttachmentAppService(
            IRepository<TFileEntity, long> fileRepository,
            IRepository<TAttachmentEntity, long> repository,
            TFileManager fileManager,
            TAttachmentManager attachmentManager,
            IBXJGAttachmentPermissionChecker attachmentPermissionChecker)
        {
            this.repository = repository;
            this.attachmentManager = attachmentManager;
            this.attachmentPermissionChecker = attachmentPermissionChecker;
            this.fileManager = fileManager;
            base.LocalizationSourceName = BXJGAttachmentConsts.LocalizationSourceName;
            this.fileRepository = fileRepository;
            this.AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }
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
        public async Task<TFileDto> CreateAsync(string tempFilePath, string md5, string fileTitle, string module, string permission, string mnemonicCode = "", string keywords = "")
        {
            await CheckPermissionAsync(module, BXJGFileOperation.Upload, permission);
            var entity = await fileManager.CreateOrGetAsync(tempFilePath, md5, fileTitle, mnemonicCode, keywords);
            return base.ObjectMapper.Map<TFileDto>(entity);
        }
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
        public async Task<TFileDto> CreateAsync(Stream stream, string md5, string fileTitle, string module, string permission, string mnemonicCode = "", string keywords = "")
        {
            await CheckPermissionAsync(module, BXJGFileOperation.Upload, permission);
            var entity = await fileManager.CreateOrGetAsync(stream, md5, fileTitle, mnemonicCode, keywords);
            return base.ObjectMapper.Map<TFileDto>(entity);
        }
        /// <summary>
        /// 检查是否可以上传。判断权限、检查文件类型、判断是否已超过当前租户允许的总磁盘容量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TCheckUploadResult> CheckUploadAsync(TCheckUploadAttachmentInput input)
        {
            var r = new TCheckUploadResult();
            try
            {
                await CheckPermissionAsync(input.Module, BXJGFileOperation.Upload, input.Permission);
            }
            catch (UserFriendlyException ex)
            {
                r.State = CheckUploadResultType.Unauthorized;
                r.Message = ex.Message;
                return r;
            }
            try
            {
                //验证规则可言考虑抽象个Pollcy接口出来
                //内部不应该直接抛出异常，而应该返回不同状态表示不同类型的错误，可以通过状态字段或不同类型Exception来实现
                await fileManager.CheckUploadAsync(input.Size, input.Extension);
            }
            catch (UserFriendlyException ex)
            {
                r.State = CheckUploadResultType.Limit;
                r.Message = ex.Message;
                return r;
            }

            var file = await fileRepository.FirstOrDefaultAsync(c => c.MD5 == input.MD5);
            if (file != null)
            {
                r.State = CheckUploadResultType.Exists;
                r.Data = ObjectMapper.Map<TFileDto>(file);
                r.Message = L("文件已存在！");
                return r;
            }
            else
            {
                r.State = CheckUploadResultType.NotExists;
                r.Message = L("验证已通过！请上传。");
                return r;
            }
        }
        /// <summary>
        /// 上传前、上传后创建附件、下载等操作时由此方法提供权限验证
        /// </summary>
        /// <param name="module">模块名</param>
        /// <param name="act">操作类型，上传/下载？</param>
        /// <param name="permission">对应的权限名</param>
        /// <returns></returns>
        protected virtual async Task CheckPermissionAsync(string module, BXJGFileOperation act, string permission)
        {
            var r = await attachmentPermissionChecker.CheckPermissionAsync(module, act, permission);
            switch (r)
            {
                case BXJGAttachmentCheckPermissionResult.IllegalRequest:
                    throw new UserFriendlyException(L("非法的附件上传请求！"));  //暂未处理本地化
                case BXJGAttachmentCheckPermissionResult.Unauthorized:
                    throw new UserFriendlyException(L("该模块的附件上传功能未授权！"));
            }
        }
        /// <summary>
        /// 下载附件。此方法先验证下载权限，若通过则将返回附件关联的文件信息和对应的流（Stream）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TDownloadOutput> DownloadAsync(TDownloadAttachmentInput input)
        {
            var c = await attachmentManager.GetWithFileByIdAsync(input.Id);//正常情况下权限都能验证过，所以这里一次性将需要的文件也查出来
            await CheckPermissionAsync(c.Module, BXJGFileOperation.Download, input.Permission);
            return new TDownloadOutput
            {
                AttachmentId = c.Id,
                Id = c.FileId,
                Extension = c.File.GetExtension(),
                Keywords = c.File.Keywords,
                MD5 = c.File.MD5,
                MnemonicCode = c.File.MnemonicCode,
                Name = c.File.Name,
                RelativePath = c.File.GetRelativePath(),
                Size = c.File.Size,
                Mime = c.File.GetMime(),
                Stream = c.File.GetStream()
            };
        }
    }

    public class BXJGAttachmentAppService : BXJGAttachmentAppService<
             BXJGFileDto,
             BXJGCheckUploadAttachmentInput,
             BXJGCheckUploadResult<BXJGFileDto>,
             BXJGDownloadAttachmentInput,
             BXJGDownloadOutput,
             BXJGFileEntty,
             BXJGAttachmentEntity,
             BXJGLocalFileManager,
             BXJGAttachmentManager>, IBXJGAttachmentAppService
    {
        public BXJGAttachmentAppService(IRepository<BXJGFileEntty, long> fileRepository, IRepository<BXJGAttachmentEntity, long> repository, BXJGLocalFileManager fileManager, BXJGAttachmentManager attachmentManager, IBXJGAttachmentPermissionChecker attachmentPermissionChecker) : base(fileRepository, repository, fileManager, attachmentManager, attachmentPermissionChecker)
        {
        }
    }
}
