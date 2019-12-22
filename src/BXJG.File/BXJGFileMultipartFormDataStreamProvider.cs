using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BXJG.File
{
    /* 普通form提交 enctype="application/x-www-form-urlencoded"
    * 带有文件上传的提交为 enctype="multipart/form-data"
    * 
    * 文件上传提交的格式与Form提交不同，如下：
    * -----------------------------114782935826962
    *   Content-Disposition: form-data; name="id"
    *
    *   WU_FILE_0
    *   -----------------------------114782935826962
    *   Content-Disposition: form-data; name="name"
    *
    *   æ æ é¢.png
    *   -----------------------------114782935826962
    *   Content-Disposition: form-data; name="type"
    *
    *   image/png
    *   -----------------------------114782935826962
    *   Content-Disposition: form-data; name="lastModifiedDate"
    *
    *   2018/12/7 ä¸å9:47:56
    *   -----------------------------114782935826962
    *   Content-Disposition: form-data; name="size"
    *
    *   51647
    *   -----------------------------114782935826962
    *   Content-Disposition: form-data; name="file"; filename="æ æ é¢.png"
    *   Content-Type: image/png
    *
    *  PNG
    *   一坨文件二进制
    * 
    * 一段代表一个数据，用分隔符分开的"-----------------------------114782935826962" ，后面一些类MimeMultipartXXX中的“MimeMultipart”应该就是表示这个每一段数据
    * 
    * **************************************************************************
    * 
    * Request.Content.ReadAsMultipartAsync关键源码：
    * 
    * //这里将streamProvider传给paser，因此读取并保存文件的动作应该是在paser中被调用的
    * using (MimeMultipartBodyPartParser parser = new MimeMultipartBodyPartParser(content, streamProvider))
    * {
    *      //读取提交来的数据并分割成小段小段的，每一段建立一个HttpConent，最后存储在streamProvider.Contents的集合中
    *      //内部调用paser读取并保存文件
    *	    await MultipartReadAsync(new MultipartAsyncContext(stream, parser, new byte[bufferSize], streamProvider.Contents), cancellationToken);
    *	    
    *	    //当streamProvider.Contents已经有数据后，做后续处理，比如读取普通Form表单数据
    *	    await streamProvider.ExecutePostProcessingAsync(cancellationToken);
    *	    return streamProvider;
    * }
    * 
    * abstract MultipartStreamProvider
    * 
    *      Collection<HttpContent> Contents;  
    *      当扩展方法Request.Content.ReadAsMultipartAsync(streamProvider);被调用时会读取上传来的数据，并将上面提到的一段对应一个HttpContent存储在此属性
    *      
    *      abstract Stream GetStream();
    *      返回一个stream，每一段数据都将写入这个流里，最终的Contents属性中的每一个HttpConent应该就是通过它得到的
    *      我们可以重写这个方法将每段数据保存到自己指定的流中去，比如内存流、网络流、文件流等
    *      
    *      ExecutePostProcessingAsync
    *      看上面的源码可知，此方法可以调用Content属性进行进一步处理，典型的是读取普通的Form表单数据
    * 
    * MultipartFileStreamProvider:MultipartStreamProvider
    *      通过重写GetStream()尝试将每一段数据保存到文件流中
    *      每一段数据对应的文件通过自己的virtual string GetLocalFileName方法提供
    *      若当前数据段不是文件类型将报错
    *      FileData表示保存后的本地文件列表
    * 
    * MultipartFormDataStreamProvider:MultipartFileStreamProvider
    *      重写GetStream()加个判断，若此段数据是文件 这直接调用父类将此段数据保存到文件流，否则返回内存流
    *      重写ExecutePostProcessingAsync，将不是文件的数据段保存未普通Form表单数据，存储在FormData属性中
    * 
    * *************************************************************************8
    * 
    * 目标：支持分片上传，非文件上传
    * 
    * 一个<input type="file" 可能支持同时选择多个文件上传，因此MultipartFormDataStreamProvider保存成功后可以通过FileData访问已经保存好的文件列表
    * 所以非分片的单文件与多文件没啥区别，下面以单文件来说明分片原理
    * 
    * 由于MultipartFormDataStreamProvider默认的读取方式接受文件流是在内存中，当接受完毕后才存入磁盘，这个倒是有方法设置，搜索"webapi大文件上传" 可以直接将接受到的文件流存储到磁盘
    * 默认上传无法实施返回上传进度，不支持断点续传
    * 
    * 分片
    * 由前端将要上传的文件切割成小块，上传时提供此小块数据md5值 + 主体文件md5值+分片序号+总分片数 等信息
    * 服务端接收到分片(对于服务端来说，每个分片都是一个普通文件)时，判断未接收完成时先暂时保持，当全部接收后合并所有分片
    * 由于分片有md5值，所以可以做到断点续传
    * 由于文件有md5值，所以可以做到秒传(若服务端本来就有对应值的文件，则认为文件一样)
    * 由于是分片，很容易做到上传进度显示
    * 
    * 由于对于服务端来说分片跟普通文件没有区别， 默认情况还是受限IIS和asp.net的文件上传大小限制
    * 
    * 通过自定义类的形式来接收分片上传来的文件（自动判断，若是小文件则不安分片形式处理)，最后进行合并 以支持大文件上传、断点续传，多线程上传等功能
    * 
    * ******************************************************************************
    * 
    * 实现思路
    * 自定义MultipartFormDataStreamProvider的子类 CustomMultipartFormDataStreamProvider
    * 
    * 要求客户端必须传输主文件md5 分片的md5
    * ===要求客户端提交时Form表单数据在前，文件在后====
    *  
    * 前端有没有必要提供chunkMd5
    * 假如同一个文件反复上传，每次分片算法一样时，就没必要传，因为每片都计算md5也是耗资源的
    * 若每次分片得到片长度或序号不同，且需要实现短点续传则由前端提供，因为要在保存前判断片是否已经存在
    * 目前考虑每次上传分片一样，且需要断点续传
    * 它是一个单独处理分片上传任务的东东，不要引入本系统和abp相关的东西
    * 若想在Controller中直接使用MultipartFormDataStreamProvider上传分片，不好做断点续传，因为是先保存文件再读Form数据，分片ID在里面
    * 
    * 关于并发--------------------------------------------
    * 另一个用户可能在上传同一个文件(md5值一样)；一次文件的上传由于分片是多线程的也可能发生并发冲突
    * 由于上传到保存文件是多个步骤，不使用分布式锁的情况下很难杜绝并发冲突，
    * 只能尽量减小冲突的可能性，
    * 当发生冲突时进行补救，
    * 补救也不行就让它错吧，因为最终有md5校验
    * 保存文件时一定要拿接收到的文件与前端传递来的md5值做对比，否则可能保存一个错误的文件
    */

    public class BXJGFileMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        #region 
        /// <summary>
        /// 当前分片数的参数名
        /// </summary>
        const string ChunkParameterName = "chunk";
        /// <summary>
        /// 总分片数的参数名
        /// </summary>
        const string ChunksParameterName = "chunks";
        /// <summary>
        /// 文件名的参数名
        /// </summary>
        const string FileParameterName = "name";
        /// <summary>
        /// 主文件MD5值的参数名
        /// </summary>
        const string FileMD5ParameterName = "filemd5";
        /// <summary>
        /// 文件大小参数名
        /// </summary>
        const string FileSizeParameterName = "size";
        /// <summary>
        /// 分片大小
        /// </summary>
        const string ChunkSizeParameterName = "chunk_size";
        ///// <summary>
        ///// 
        ///// </summary>
        //const string ModuleParameterName = "module";
        #endregion

        #region 属性
        /// <summary>
        /// 获取分片上传时的片数
        /// </summary>
        public int Chunks { get; private set; }
        /// <summary>
        /// 获取分片上传时的片序号
        /// </summary>
        public int Chunk { get; private set; }
        /// <summary>
        /// 是否是分片
        /// </summary>
        public bool IsChunk { get; private set; }
        /// <summary>
        /// 获取主文件的md5值
        /// </summary>
        public string FileMD5 { get; private set; }
        ///// <summary>
        ///// 组成临时文件目录的一部分 临时目录还包括构造函数中的uid
        ///// 组成部分越多 出现并发冲突的几率越小
        ///// </summary>
        //public string Module { get; private set; }
        /// <summary>
        /// 获取主文件名
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// 获取当前上传文件的临时目录
        /// 主文件md5_Module_uid
        /// 组成部分越多 出现并发冲突的几率越小
        /// </summary>
        public string TempDirectory { get; private set; }
        /// <summary>
        /// 文件大小 单位字节
        /// </summary>
        public long FileSize { get; private set; }
        /// <summary>
        /// 分片大小 单位字节
        /// </summary>
        public long ChunkSize { get; private set; }
        ///// <summary>
        ///// 合并时发生冲突
        ///// </summary>
        //public bool MergeConflict { get; private set; }
        #endregion



        public BXJGFileMultipartFormDataStreamProvider(string path) : base(path)
        {
        }

        public override async Task ExecutePostProcessingAsync()
        {
            await base.ExecutePostProcessingAsync(); //填充FormData属性(表单数据)

            if (!IsChunk)
                return;

            //最后几个分片可能同时上了。多个用户可能上传同一个文件，都可能造成并发冲突
            //目前这种合并方式是相对好点的

            string[] files = Directory.GetFiles(TempDirectory);
            if (Chunk != Chunks - 1 || files.Length != Chunks)
                return;

            var newFileName = CreateFilePath();
            FileStream stream = null;
            try
            {
                stream = new FileStream(newFileName, FileMode.CreateNew);
            }
            catch (IOException ex)
            {
                if (ex.HResult != -2147024816)
                    throw;

                //Logger.WarnFormat("分片上传文件时创建文件流失败：" + ex.Message);
                return;
            }

            using (stream)
            {
                var sortedFiles = files.OrderBy(c => int.Parse(Path.GetFileNameWithoutExtension(c)));
                await Task.Run(async () =>
                {
                    foreach (var item in sortedFiles)
                    {
                        #region MyRegion
                        //此时可能另一个分片并没有读取完成
                        for (int i = 0; i < 10000; i++)
                        {
                            try
                            {
                                using (var fs = System.IO.File.OpenRead(item))
                                {
                                    fs.CopyTo(stream);
                                }
                                break;
                            }
                            catch (IOException ex)
                            {
                                if (ex.HResult != -2147024864)
                                    throw;
                                await Task.Delay(2);
                            }
                        }
                        #endregion
                    }
                });
            }
            var lastCT = FileData.First();

            var heders = lastCT.Headers;

            base.FileData.Clear();

            FileData.Add(new MultipartFileData(heders, newFileName));
            Directory.Delete(TempDirectory, true);
            IsChunk = false;
        }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            if (string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName))
                return new MemoryStream();//除了文件，其它的普通表单数据都使用内存流

            //假定文件流是表单的其它数据之后，只考虑一次来单个文件

            FileName = GetFormValue(FileParameterName);
            FileMD5 = GetFormValue(FileMD5ParameterName);
            FileSize = long.Parse(GetFormValue(FileSizeParameterName));

            var strChunks = GetFormValue(ChunksParameterName);
            #region 非分片上传
            if (strChunks.IsNullOrWhiteSpace())
            {
                var localFileName1 = CreateFilePath();
                FileData.Add(new MultipartFileData(headers, localFileName1));

                //参考顶部注释关于并发的解释
                //由于是非分片，发生并发冲突是直接抛出异常阻止继续上传
                if (System.IO.File.Exists(localFileName1))
                {
                    if (new FileInfo(localFileName1).Length < FileSize)
                        return System.IO.File.Create(localFileName1);
                    else
                        return new InnerStream();
                }
                else
                    return new FileStream(localFileName1, FileMode.CreateNew);
            }
            #endregion

            #region 分片上传
            Chunks = int.Parse(strChunks);//总片数
            Chunk = int.Parse(GetFormValue(ChunkParameterName));//当前片序号
            ChunkSize = long.Parse(GetFormValue(ChunkSizeParameterName));
            IsChunk = true;
            TempDirectory = Path.Combine(RootPath, FileMD5);//分片要保存到的临时目录

            if (!Directory.Exists(TempDirectory))
                Directory.CreateDirectory(TempDirectory);

            //参考顶部注释关于并发的解释
            var fp = Path.Combine(TempDirectory, Chunk.ToString());

            FileData.Add(new MultipartFileData(headers, fp));

            //参考顶部注释关于并发的解释
            //由于是分片，发生并发时尽可能补救
            try
            {
                if (System.IO.File.Exists(fp))
                {
                    if (new FileInfo(fp).Length < ChunkSize)
                        return System.IO.File.Create(fp);//-2147024864
                    else
                        return new InnerStream();
                }
                else
                    return new FileStream(fp, FileMode.CreateNew);//-2147024816
            }
            catch (IOException ex)
            {
                if (ex.HResult == -2147024864 || ex.HResult == -2147024816)
                    return new InnerStream();

                throw;
            }
            #endregion
        }
        /// <summary>
        /// 从请求中获取指定表单数据
        /// 若没找到则返回null
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        string GetFormValue(string p)
        {
            var headers = Contents.SingleOrDefault(c => c.Headers.ContentDisposition.Name.Trim('\"').Equals(p, StringComparison.OrdinalIgnoreCase));
            if (headers == null)
                return null;
            return headers.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        /// <summary>
        /// 在RootPath中创建最终文件
        /// RootPath+guid+扩展名  调用之前先确保RootPath、FileName已赋值
        /// </summary>
        /// <returns></returns>
        string CreateFilePath()
        {
            return Path.Combine(RootPath, FileMD5 + Path.GetExtension(FileName));
        }
        /// <summary>
        /// 辅助类，只读文件流
        /// 当分片已存在时返回此对象，辅助实现断点续传
        /// </summary>
        class InnerStream : MemoryStream
        {
            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return Task.FromResult<object>(null);
                //return Task.FromCanceled(cancellationToken);
            }
            public override void Write(byte[] array, int offset, int count)
            {
                //base.Write(array, offset, count);
            }
            public override void WriteByte(byte value)
            {
                //base.WriteByte(value);
            }
        }
    }
}
