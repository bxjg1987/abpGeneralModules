using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BXJG.File;

namespace BXJG.Attachment
{
    //领域服务根据情况而定，可以再抽象出接口来。目前简单起见 没有做抽象

    /// <summary>
    /// 附件的领域服务
    /// </summary>
    public class BXJGAttachmentManager<TFile, TAttachment, TFileManager> : DomainService
        where TFile : BXJGFileEntty, new()
        where TAttachment : BXJGAttachmentEntity<TFile>, new()
        where TFileManager : BXJGLocalFileManager<TFile>
    {
        //protected readonly IAttachmentPermissionChecker AttachmentPermissionChecker;//权限判断放应用层

        private readonly IRepository<TFile, long> fileRepository;
        private readonly IRepository<TAttachment, long> repository;
        private readonly TFileManager fileManager;

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }//属性注入 要public

        //限制大小放到LocalFileManager中，如果附件有特殊要求 再在这里来做限制

        public BXJGAttachmentManager(IRepository<TFile, long> fileRepository, IRepository<TAttachment, long> repository, TFileManager localFileManager)
        {
            base.LocalizationSourceName = BXJGAttachmentConsts.LocalizationSourceName;
            this.fileRepository = fileRepository;
            this.repository = repository;
            this.fileManager = localFileManager;
            this.AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }
        /// <summary>
        /// 为指定模块下的指定id的对象设置附件
        /// </summary>
        /// <param name="module">模块，如：EquipmentInfo</param>
        /// <param name="objId">实体id,如：某个具体设备信息的Id</param>
        /// <param name="fileIds">关联的文件的id集合</param>
        /// <returns></returns>
        public async Task<IList<TAttachment>> SetAttachmentAsync(string module, object objId, params long[] fileIds)
        {
            //获取该对象原有的所有附件
            //保留当前设置的，删除多余的附件，不仅仅是删除关系，而是直接删除附件
            //int[] oldArray = { 1, 2, 3, 4, 5 };
            //int[] newArray = { 2, 4, 5, 7, 8, 9 };
            //var jiaoJi = oldArray.Intersect(newArray).ToList();   //2,4,5
            //var oldChaJi = oldArray.Except(newArray).ToList();    //1,3
            //var newChaJi = newArray.Except(oldArray).ToList();    //7,8,9
            //var bingJi = oldArray.Union(newArray).ToList();       //1,2,3,4,5,7,8,9

            var oid = objId.ToString();
            if (fileIds == null)
                fileIds = new long[] { };

            //原来的【附件】集合，其中包含文件
            var oldAttachments = await AsyncQueryableExecuter.ToListAsync(
                    repository.GetAll().Where(c => c.Module == module && c.ObjectId == oid));
            var oldFileids = oldAttachments.Select(c => c.FileId);      //原来的文件ids集合 目前的设计附件与文件是一对一关系

            var deleteFileIds = oldFileids.Except(fileIds);             //要删除的文件id集合
            foreach (var item in deleteFileIds)
            {
                var att = oldAttachments.Single(c => c.FileId == item);
                await repository.DeleteAsync(att);
                //await fileManager.DeleteAsync(att.File);
            }

            var insertFileIds = fileIds.Except(oldFileids);             //要新增的文件id集合
            foreach (var item in insertFileIds)
            {
                var entity = new TAttachment
                {
                    Module = module,
                    FileId = item,
                    ObjectId = oid
                };
                await repository.InsertAsync(entity);
            }
            CurrentUnitOfWork.SaveChanges();
            return await GetAttachmentAsync(module, oid);
        }
        /// <summary>
        /// 获取指定模块下指定对象的附件列表(包含关联的文件信息)
        /// </summary>
        /// <param name="module">模块，如：EquipmentInfo</param>
        /// <param name="objId">实体id,如：某个具体设备信息的Id</param>
        /// <returns></returns>
        public async Task<IList<TAttachment>> GetAttachmentAsync(string module, object objId)
        {
            var oid = objId.ToString();
            return await AsyncQueryableExecuter.ToListAsync(
                repository.GetAllIncluding(c => c.File).Where(c => c.Module == module && c.ObjectId == oid)
            );
        }
        /// <summary>
        /// 获取附件及其文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TAttachment> GetWithFileByIdAsync(long id)
        {
            //由于附件的Id是主键，因此这里使用FirstOrDefaultAsync跟SingleOrDefaultAsync无差别
            return AsyncQueryableExecuter.FirstOrDefaultAsync(repository.GetAllIncluding(c => c.File).Where(c => c.Id == id));
        }
    }

    public class BXJGAttachmentManager : BXJGAttachmentManager<BXJGFileEntty, BXJGAttachmentEntity, BXJGLocalFileManager>
    {
        public BXJGAttachmentManager(IRepository<BXJGFileEntty, long> fileRepository, IRepository<BXJGAttachmentEntity, long> repository, BXJGLocalFileManager localFileManager) : base(fileRepository, repository, localFileManager)
        {
        }
    }
}
