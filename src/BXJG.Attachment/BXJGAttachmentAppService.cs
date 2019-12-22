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

namespace BXJG.Attachment
{
    public class BXJGAttachmentAppService<
         TFileDto,
         TCheckUploadAttachmentInput,
         TCheckUploadResult,
         TDownloadAttachmentInput,
         TDownloadOutput,
         TFileEntity,
         TAttachmentEntity,
         TFileManager,
         TAttachmentManager> : ApplicationService,
        IAttachmentAppService<TFileDto, TCheckUploadAttachmentInput, TCheckUploadResult, TDownloadAttachmentInput, TDownloadOutput>
        where TCheckUploadAttachmentInput : CheckUploadAttachmentInput
        where TCheckUploadResult : CheckUploadResult<TFileDto>, new()
        where TDownloadAttachmentInput : DownloadAttachmentInput
        where TDownloadOutput : DownloadOutput, new()
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

        public async Task<TFileDto> CreateAsync(string tempFilePath, string md5, string fileTitle, string module, string permission, string mnemonicCode = "", string keywords = "")
        {
            await CheckPermissionAsync(module, BXJGFileOperation.Upload, permission);
            var entity = await fileManager.CreateOrGetAsync(tempFilePath, md5, fileTitle, mnemonicCode, keywords);
            return base.ObjectMapper.Map<TFileDto>(entity);
        }

        public async Task<TFileDto> CreateAsync(Stream stream, string md5, string fileTitle, string module, string permission, string mnemonicCode = "", string keywords = "")
        {
            await CheckPermissionAsync(module, BXJGFileOperation.Upload, permission);
            var entity = await fileManager.CreateOrGetAsync(stream, md5, fileTitle, mnemonicCode, keywords);
            return base.ObjectMapper.Map<TFileDto>(entity);
        }

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

        protected async Task CheckPermissionAsync(string module, BXJGFileOperation act, string permission)
        {
            var r = await attachmentPermissionChecker.CheckPermission(module, act, permission);
            switch (r)
            {
                case BXJGAttachmentCheckPermissionResult.IllegalRequest:
                    throw new UserFriendlyException(L("非法的附件上传请求！"));  //暂未处理本地化
                case BXJGAttachmentCheckPermissionResult.Unauthorized:
                    throw new UserFriendlyException(L("该模块的附件上传功能未授权！"));
            }
        }

        public async Task<TDownloadOutput> DownloadAsync(TDownloadAttachmentInput input)
        {
            var c = await attachmentManager.GetWithFileByIdAsync(input.Id);//正常情况下权限都能验证过，所以这里一次性将需要的文件也查出来
            await CheckPermissionAsync(c.Module, BXJGFileOperation.Download, input.Permission);
            return new TDownloadOutput
            {
                AttachmentId = c.Id,
                Id = c.FileId,
                Extension = c.File.Extension,
                Keywords = c.File.Keywords,
                MD5 = c.File.MD5,
                MnemonicCode = c.File.MnemonicCode,
                Name = c.File.Name,
                RelativePath = c.File.RelativePath,
                Size = c.File.Size,
                Mime = c.File.Mime,
                Stream = c.File.GetStream()
            };
        }
    }

    public class BXJGAttachmentAppService : BXJGAttachmentAppService<
             BXJGFileDto,
             CheckUploadAttachmentInput,
             CheckUploadResult<BXJGFileDto>,
             DownloadAttachmentInput,
             DownloadOutput,
             BXJGFileEntty,
             BXJGAttachmentEntity,
             BXJGLocalFileManager,
             BXJGAttachmentManager>
    {
        public BXJGAttachmentAppService(IRepository<BXJGFileEntty, long> fileRepository, IRepository<BXJGAttachmentEntity, long> repository, BXJGLocalFileManager fileManager, BXJGAttachmentManager attachmentManager, IBXJGAttachmentPermissionChecker attachmentPermissionChecker) : base(fileRepository, repository, fileManager, attachmentManager, attachmentPermissionChecker)
        {
        }
    }
}
