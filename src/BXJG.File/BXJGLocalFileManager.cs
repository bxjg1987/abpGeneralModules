using Abp.Application.Features;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Extensions;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Linq;

namespace BXJG.File
{
    /*
     * 如果一个操作发生异常时，不希望记录错误日志，希望直接显示给用户错误原因，应该抛出UserFrendlyException，否则抛出ApplicationException
     * ABPException应该是ABP框架抛出的异常
     * 
     * 权限检查应放在应用服务层
     * 
     * 限制大小、后缀等规则
     * 定义一个附件规则接口，继承ABP提供的IPolicy\UserPolicy(asp.net zero源码中)
     * 定义多个实现类，每一个实现类一个限制条件，比如实现类A限制大小 实现类B限制后缀
     * 定义一个集合（也实现附件规则接口） 容纳这些附件规则，然后在这个集合上实现附件规则接口的方法，遍历内部每一个规则 做最终验证
     * 目前简单起见 直接在这个领域服务实现后缀和大小验证
     * 
     * 关于并发
     * 在上传、下载的瞬间另一个线程删除此文件或记录
     * 最保险的方式是不删除文件，但是可能造成大量无用的文件占用磁盘空间
     * 无用的文件分两种，一种是文件软删除，意思是数据库记录删了，物理文件还存在
     * 另一种是数据库记录和物理文件都存在，但是没有被任何地方引用
     * 
     * 上传           下载          软删除               删除
     * 保存文件       查询记录      软删除记录           删除记录
     * 保存记录       返回文件      不处理文件           删除文件
     * 
     * 若文件和记录都存在，即使没有被引用也不处理
     * 不考虑支持软删除，因为为了保持文件唯一使用md5作为文件名，若考虑软删除情况会很复杂，因此任何地方需要删除文件都应该调用LocalFileManager.Delte，而不能直接使用Repository
     * 下载虽然可能因为另一个线程删除文件导致异常，但是它只是查询，即使发生异常也无所谓
     * 最终剩下如下问题
     * 
     * 用户A：
     * 上传              删除
     * 1保存文件         3删除记录
     * 2保存记录         4删除文件
     * 
     * 用户B：
     * 上传              删除
     * 5保存文件         7删除记录
     * 6保存记录         8删除文件
     * 
     * 假定两个用户操作的文件的md5值一样
     * 此时每种操作都是两个步骤，并发冲突可能性较大
     * 1 8 2 这种执行顺序就出现问题了
     * 
     * 单体架构我们可以考虑集合或Dictionary来为每一个文件实现锁操作（不能用内存缓存，怕掉）
     * 分布式情况下最简单的请是使用分布式锁
     * 
     * 换个思路，既然上面说了当文件和记录都存在时，即使文件没有被任何地方引用我们也不做删除，因为这些文件将来可能被引用
     * 所以现在只考虑一种情况， 上传时，文件已保存，但是在插入数据库记录时失败了
     * 一种办法是插入数据库失败时直接再删物理文件，但是这一步可能出错 没有删除成功
     * 最终导致下次上传这个文件是永远无法成功
     * 另一种办法是定时作业进行删除那些有物理文件却没有数据库记录的文件
     * 但是可能存在一个线程正在上传，还没来得及做数据库记录时，刚好物理文件被定时任务删除了
     * 
     * 最终方案
     * 
     * 上传改为3个步骤：
     * 
     * 上传前查看标记为2的认为是已存在
     * 标记为1的认为是不存在
     * 
     * 
     * 1、插入数据库记录，标记字段1
     * 2、保存物理文件
     * 3、更新数据库标记字段为2
     * 
     * 
     * ----------暂时不考虑删除
     * 删除改为3个步骤
     * 1、更新数据库记录标记为3
     * 2、删除物理文件
     * 3、删除数据库记录
     * ----------暂时不考虑删除
     * 
     * 记录的md5保持唯一约束
     * 
     * 定时任务
     * 查找表示字段1的记录
     * 若文件存在则删除，若删除成功再删除数据库记录
     * 若文件不存在则直接删除记录
     * 
     * 
     * 
     * 想得太复杂了
     * 上传还是保持原来的两个步骤，保存文件 增添数据库记录
     * 定时任务直接删除那些没有记录但是存在物理文件的文件
     * 不提供删除功能
     * （要实现这个 严格来说应该单独定义一个IRepository<BXJGFileEntty, long>的实现类，并注册到容器，这样调用方即便注入IRepository<BXJGFileEntty, long>调用Delete也无法删除记录）
     * 
     */

    /// <summary>
    /// 本地文件管理领域逻辑类
    /// </summary>
    public class BXJGLocalFileManager<TEntity> : DomainService//, IFileManager //现在对网盘接口还不熟悉，以后再考虑封装吧
   where TEntity: BXJGFileEntty, new()
    {
        private readonly IRepository<TEntity, long> repository;
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }//属性注入 要public
        private readonly IFeatureChecker featureChecker;

        public BXJGLocalFileManager(IRepository<TEntity, long> repository, IFeatureChecker featureChecker)
        {
            base.LocalizationSourceName = BXJGFileConsts.LocalizationSourceName;
            this.repository = repository;
            this.featureChecker = featureChecker;
            this.AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }
        /// <summary>
        /// 创建一个文件
        /// <para>上传文件时先将文件存储到一个临时路径下，然后调用此方法会将文件从临时路径移动到指定位置，并做数据记录</para>
        /// <para>可能因为并发问题失败(md5有数据库唯一约束)，可能性不大，因为会先查下，若存在就直接返回</para>
        /// <para>可能因为定时任务删除操作，虽然返回有值，但是实际记录和文件已经被删除(有TEntity.ReferenceLastTime控制，可能性很小)</para>
        /// </summary>
        /// <param name="tempFilePath">上传来的文件存放的临时路径</param>
        /// <param name="md5">文件的md5值，若不传则取tempFilePath对应文件的md5</param>/// <param name="extName">如：.txt</param>
        /// <param name="dir">希望保存到哪个目录，完整路径为：AppDomain.CurrentDomain.BaseDirectory + Consts.UploadRoot + path + Path.GetFileName(tempFilePath)</param>
        /// <param name="fileTitle">文件的标题，对应TEntity.Name，如：aaa.txt</param>
        /// <param name="mnemonicCode">此文件的助记码，如拼音码 方便搜索时使用</param>
        /// <param name="keywords">文件关联的关键字，便于搜索</param>
        /// <returns></returns>
        public async Task<TEntity> CreateOrGetAsync(string tempFilePath, string md5, string fileTitle, string mnemonicCode = null, string keywords = null)
        {
            /*
             * 移动时若目标文件已存在会异常
             * 插入时由于md5有唯一约束，若存在也会异常
             * 无论是直接查询或插入后 最终返回的文件都不一定是真是存在的，因为另一个线程的任务可能刚好在返回前删除了此文件和记录
             */

            var extName = Path.GetExtension(tempFilePath);
            if (extName.IsNullOrWhiteSpace())
                extName = Path.GetExtension(fileTitle);
            //if (!extName.StartsWith("."))
            //    extName = "." + extName;

            var fmd5 = tempFilePath.GetMD5ByFilePath();
            //由于webuploader存在bug，后缀为jpg的文件上传前貌似会修改，导致上传后的文件与源文件md5匹配失败，
            //因此这里暂时跳过.jpg的md5校验
            if (!extName.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                if (!fmd5.Equals(md5, StringComparison.OrdinalIgnoreCase))
                    throw new UserFriendlyException("MD5校验失败！");
            }

            FileInfo localFile = new FileInfo(tempFilePath);
            await CheckUploadAsync(localFile.Length, extName); //检查大小和后缀限制

            var fe = await repository.FirstOrDefaultAsync(c => c.MD5 == md5);// GetByMD5Async(md5);
            if (fe != null)
                return fe;

            //fileTitle = Path.GetFileNameWithoutExtension(fileTitle);

            //文件的相对路径和绝对路径
            string path = null, targetPath = null;

            var fileName = md5 + extName;//a.txt

            path = Path.Combine(BXJGFileConsts.dir, fileName);//数据库目录   equipmentinfo\a.txt

            var targetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BXJGFileConsts.UploadRoot, BXJGFileConsts.dir);  //d:\abp\uploads\equipmentinfo\
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            targetPath = Path.Combine(targetDir, fileName);

            System.IO.File.Move(tempFilePath, targetPath);

            fe = new TEntity();
            fe.Name = fileTitle;
            fe.Path = path;
            fe.Keywords = keywords;
            fe.Size = localFile.Length;
            fe.MD5 = md5;
            //fe.RefresReference();// = DateTime.Now;
            await repository.InsertAsync(fe);
            await CurrentUnitOfWork.SaveChangesAsync();
            return fe;
        }
        /// <summary>
        /// 创建一个文件
        /// <para>上传文件时先将文件存储到一个临时路径下，然后调用此方法会将文件从临时路径移动到指定位置，并做数据记录</para>
        /// <para>可能因为并发问题失败(md5有数据库唯一约束)，可能性不大，因为会先查下，若存在就直接返回</para>
        /// <para>可能因为定时任务删除操作，虽然返回有值，但是实际记录和文件已经被删除(有TEntity.ReferenceLastTime控制，可能性很小)</para>
        /// </summary>
        /// <param name="stream">要保存的文件流</param>
        /// <param name="md5">文件的md5值，若不传则取tempFilePath对应文件的md5</param>/// <param name="extName">如：.txt</param>
        /// <param name="dir">希望保存到哪个目录，完整路径为：AppDomain.CurrentDomain.BaseDirectory + Consts.UploadRoot + path + Path.GetFileName(tempFilePath)</param>
        /// <param name="fileTitle">文件的标题，对应TEntity.Name，如：aaa.txt</param>
        /// <param name="mnemonicCode">此文件的助记码，如拼音码 方便搜索时使用</param>
        /// <param name="keywords">文件关联的关键字，便于搜索</param>
        /// <returns></returns>
        public async Task<TEntity> CreateOrGetAsync(Stream stream, string md5, string fileTitle, string mnemonicCode = null, string keywords = null)
        {
            var extName = Path.GetExtension(fileTitle);
            var fmd5 = stream.GetMD5();
            //由于webuploader存在bug，后缀为jpg的文件上传前貌似会修改，导致上传后的文件与源文件md5匹配失败，因此这里暂时跳过.jpg的md5校验
            if (!extName.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                if (!fmd5.Equals(md5, StringComparison.OrdinalIgnoreCase))
                    throw new UserFriendlyException("MD5校验失败！");
            }

            await CheckUploadAsync(stream.Length, extName); //检查大小和后缀限制

            var fe = await repository.FirstOrDefaultAsync(c => c.MD5 == md5);// GetByMD5Async(md5);
            if (fe != null)
                return fe;

            //fileTitle = Path.GetFileNameWithoutExtension(fileTitle);
            var fileName = md5 + extName;

            var path = Path.Combine(BXJGFileConsts.dir, fileName);//数据库目录

            var targetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BXJGFileConsts.UploadRoot, BXJGFileConsts.dir);
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            var targetPath = Path.Combine(targetDir, fileName);
            using (var fs = new FileStream(targetPath, FileMode.CreateNew, FileAccess.Write))
            {
                await stream.CopyToAsync(fs);
            }

            fe = new TEntity();
            fe.Name = fileTitle;
            fe.Keywords = keywords;
            fe.Size = stream.Length;
            fe.MD5 = md5;
            fe.Path = path;
            //fe.RefresReference();
            await repository.InsertAsync(fe);
            await CurrentUnitOfWork.SaveChangesAsync();
            return fe;
        }
        /// <summary>
        /// 批量软删除指定id的记录，并不会删除处理文件
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<IList<long>> DeleteAsync(params long[] ids)
        {
            var files = await repository.GetAllListAsync(c => ids.Contains(c.Id));
            return await DeleteAsync(files.ToArray());
        }
        /// <summary>
        /// 批量硬删除指定id的记录及其关联的物理文件
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<IList<long>> HardDeleteAsync(params long[] ids)
        {
            var files = await repository.GetAllListAsync(c => ids.Contains(c.Id));
            return await HardDeleteAsync(files.ToArray());
        }
        /// <summary>
        /// 批量软删除指定的记录
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<IList<long>> DeleteAsync(params TEntity[] files)
        {
            var list = new List<long>();
            var ts = new List<Task>();
            foreach (var item in files)
            {
                //经过测试.net 2.0 foreach都不存在闭包问题
                //无论文件是否被引用存在外键，软删除都会成功
                var t = repository.HardDeleteAsync(item).ContinueWith(t1 =>
                {
                    list.Add(item.Id);//软删除成功时记录对应文件的id，以便返回

                }, TaskContinuationOptions.NotOnFaulted).ContinueWith(c =>
                {
                    //若失败，则记录错误 并终止异常抛出，以便继续处理下一条
                    c.Exception.Handle(d =>
                    {
                        Logger.ErrorFormat(d, "软删除文件失败{0}", item.Id);
                        return true;//true表示已经处理了异常，不要继续抛出了
                    });
                }, TaskContinuationOptions.OnlyOnFaulted);
                ts.Add(t);
            }

            await Task.WhenAll(ts);//这样搞 好像好报错
            return list;
        }
        /// <summary>
        /// 批量硬删除指定id的记录及其关联的物理文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<IList<long>> HardDeleteAsync(params TEntity[] files)
        {
            var list = new List<long>();
            var ts = new List<Task>();
            foreach (var item in files)
            {
                //经过测试.net 2.0 foreach都不存在闭包问题
                //若文件被引用，外键关系可能导致删除失败
                var t = repository.HardDeleteAsync(item).ContinueWith(t1 =>
                {
                    list.Add(item.Id);
                   System.IO. File.Delete(item.GetAbsolutePath());

                }, TaskContinuationOptions.NotOnFaulted).ContinueWith(c =>
                {
                    c.Exception.Handle(d =>
                    {
                        Logger.ErrorFormat(d, "硬删除文件失败{0}", item.Id);
                        return true;//true表示已经处理了异常，不要继续抛出了
                    });
                }, TaskContinuationOptions.OnlyOnFaulted);
                ts.Add(t);
            }
            await Task.WhenAll(ts);//这样搞 好像好报错
            return list;
        }
        /// <summary>
        /// 软删除指定目录下的所有文件
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="hard"></param>
        /// <returns></returns>
        public async Task<IList<long>> DeleteByDirAsync(string dir, bool hard = false)
        {
            if (!dir.EndsWith("\\"))
                dir = dir + "\\";
            var files = await repository.GetAllListAsync(c => c.Path.StartsWith(dir));
            if (hard)
                return await HardDeleteAsync(files.ToArray());
            else
                return await DeleteAsync(files.ToArray());
        }
        /// <summary>
        /// 根据物理文件刷新数据库中的记录，主要是大小、md5值、最后修改时间等
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> RefreshAsync(long id)
        {
            var entity = await repository.GetAsync(id);
            entity.RefreshAsync();
            await repository.UpdateAsync(entity);
            return entity;
        }
        /// <summary>
        /// 根据物理文件刷新数据库中的记录，主要是大小、md5值、最后修改时间等
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> RefreshByDirAsync(string dir)
        {
            if (!dir.EndsWith("\\"))
                dir = dir + "\\";
            var files = await repository.GetAllListAsync(c => c.Path.StartsWith(dir));
            var ts = new List<Task>();
            foreach (var item in files)
            {
                var q = item;
                item.RefreshAsync();

                var t = repository.UpdateAsync(item).ContinueWith(c =>
                {
                    c.Exception.Handle(d =>
                    {
                        Logger.ErrorFormat(d, "刷新文件失败{0}", q.Id);
                        return true;
                    });
                }, TaskContinuationOptions.OnlyOnFaulted);

                ts.Add(t);
            }
            await Task.WhenAll(ts);
            return files;
        }
        /// <summary>
        /// 获取当前租户已上传文件的总大小Mb
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetSizeTotalAsync()
        {
            // var total = repository.GetAll().Sum(c => c.Size);
            //Task.Run这样写有可能会出现问题
            var total = await Task.Run(() => repository.GetAll().Select(c => c.Size).DefaultIfEmpty().Sum());
            return total / 1024 / 1024;
        }

        //public Task MoveAsync(TEntity file, string dir)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task MoveAsync(long fileId, string dir)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// 检查限制 大小(目前只检查总大小) 文件类型等
        /// </summary>
        /// <param name="size">文件大小Kb</param>
        /// <param name="hz">文件后缀名，如：.txt</param>
        /// <returns></returns>
        public async Task CheckUploadAsync(long size, string hz)
        {
            //当前租户的总文件大小限制，包含附件，因为附件也使用的文件管理
            var fsize = (await featureChecker.GetValueAsync(BXJGFileConsts.MaxFileUploadSizeFeature)).To<long>();    //当前租户对应(版本)限制最大文件容量，单位Mb
            var currTotal = await GetSizeTotalAsync();//两个异步操作应该可以通过Task.WhenAll来实现更快的查询

            //var t1 = featureChecker.GetValueAsync(BXJGConsts.MaxFileUploadSizeFeature);
            //var t2 = GetSizeTotalAsync();
            //await Task.WhenAll(t1, t2);

            //long fsize = t2.Result;
            //long currTotal = t1.To<long>();

            if (currTotal >= fsize)
                throw new UserFriendlyException(L("上传文件的总大小超过限制"), L("允许上传{0}Mb，实际已上传{1}Mb", fsize, currTotal));

            var setting = await SettingManager.GetSettingValueAsync(BXJGFileConsts.FileUploadExtensionSetting);
            if (!hz.StartsWith("."))
                hz = "." + hz;
            hz = hz.ToLower();
            var ary = setting.Split(' ');
            if (!ary.Any(c => c.ToLower().Trim() == hz))
                throw new UserFriendlyException(L("不支持的上传文件的类型"), L("允许的类型为：{0}", setting));
        }

        ///// <summary>
        ///// 根据md5获取文件，若没找到则返回null
        ///// 内部将刷新最后引用时间
        ///// </summary>
        ///// <param name="md5"></param>
        ///// <returns></returns>
        //public async Task<TEntity> GetByMD5Async(string md5)
        //{
        //    var entity = await repository.FirstOrDefaultAsync(c => c.MD5 == md5);
        //    if (entity != null)
        //    {
        //        entity.RefresReference();
        //        await repository.UpdateAsync(entity);
        //    }
        //    return entity;
        //}
        ///// <summary>
        ///// 根据id获取文件，若没找到则返回null.内部将刷新最后引用时间
        ///// </summary>
        ///// <param name="id">文件id</param>
        ///// <returns></returns>
        //public async Task<TEntity> GetByIdAsync(long id)
        //{
        //    var entity = await repository.FirstOrDefaultAsync(c => c.Id == id);
        //    if (entity != null)
        //    {
        //        entity.RefresReference();
        //        await repository.UpdateAsync(entity);
        //    }
        //    return entity;
        //}
    }

    public class BXJGLocalFileManager : BXJGLocalFileManager<BXJGFileEntty>
    {
        public BXJGLocalFileManager(IRepository<BXJGFileEntty, long> repository, IFeatureChecker featureChecker) : base(repository, featureChecker)
        {
        }
    }
}
