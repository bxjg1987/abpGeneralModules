using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    //************************EF6 linq中不支持泛型属性的操作************************

    /// <summary>
    /// 通用树形领域服务
    /// </summary>
    /// <typeparam name="TEntity">通用树形实体类型</typeparam>
    public abstract class GeneralTreeManager<TEntity> : DomainService
        where TEntity : GeneralTreeEntity<TEntity>
    {
        protected IRepository<TEntity, long> repository;

        //属性注入，需要public
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public GeneralTreeManager(IRepository<TEntity, long> repository)
        {
            this.repository = repository;
            base.LocalizationSourceName = GeneralTreeConsts.LocalizationSourceName;
            this.AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            if (entity.ParentId.HasValue)
            {
                var parentCodeQuery = repository.GetAll().Where(c => c.Id == entity.ParentId.Value).Select(c => c.Code);
                var parentCode = await AsyncQueryableExecuter.FirstOrDefaultAsync(parentCodeQuery);
                var childrenCount = await repository.CountAsync(c => c.ParentId == entity.ParentId);
                entity.Code = BuildCode(parentCode, childrenCount);
            }
            else
            {
                var childrenCount = await repository.CountAsync(c => c.ParentId == null);
                entity.Code = BuildCode("", childrenCount);
            }
            await repository.InsertAsync(entity);
            await base.CurrentUnitOfWork.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            //var o = await repository.GetAsync(entity.Id);
            //o.ExtensionData = entity.ExtensionData;
            //o.DisplayName = entity.DisplayName;


            long? newParentId = entity.ParentId;

            var oldQuery = repository.GetAll().Where(c => c.Id == entity.Id).Select(c => c.ParentId);
            var old = await AsyncQueryableExecuter.FirstOrDefaultAsync(oldQuery);
            var needMove = old != entity.ParentId;
            //entity.Code = old.Code;
            entity.ParentId = old;

            await repository.UpdateAsync(entity);
            if (needMove)
            {
                if (newParentId.HasValue)
                    await MoveAsync(entity, await repository.GetAsync(newParentId.Value), GeneralTreeMoveType.Append);
                else
                {
                    var ds = await AsyncQueryableExecuter.FirstOrDefaultAsync(repository.GetAll().Where(c => c.ParentId == null).OrderByDescending(c => c.Code));
                    await MoveAsync(entity, ds, GeneralTreeMoveType.After);
                }
            }
            return entity;
        }

        public virtual async Task DeleteAsync(params long[] keys)
        {
            if (keys != null && keys.Length > 0)
            {
                //默认情况下ParentId的外键关联没有做级联删除，因为我们想通过程序来控制
                var entities = await repository.GetAllListAsync(c => keys.Contains(c.Id));
                foreach (var item in entities)
                {
                    await repository.DeleteAsync(c => c.Code.StartsWith(item.Code));
                }
            }
            else
                await repository.DeleteAsync(c => true);
        }

        //public virtual async Task<TEntity> GetEntityWithOffspringAsync(long id)
        //{
        //    var parent = await repository.GetAsync(id);
        //    var list = await GetFlattenOffspringAsync(parent.Code);
        //    return parent;//ef自动建立父子关系，
        //}
        /// <summary>
        /// 移动节点
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="targetId"></param>
        /// <param name="moveType"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> MoveAsync(long sourceId, long? targetId, GeneralTreeMoveType moveType)
        {
            if (targetId == 0)
                targetId = null;
            var source = await repository.GetAsync(sourceId);
            var target = targetId.HasValue ? await repository.GetAsync(targetId.Value) : null;
            return await MoveAsync(source, target, moveType);
        }
        public virtual async Task<TEntity> MoveAsync(TEntity source, TEntity target, GeneralTreeMoveType moveType)
        {

            TEntity targetParent = null;
            IList<TEntity> targetChildren = null;
            int targetIndex = 0;

            //if (target == null)
            //{
            //    //if (moveType != GeneralTreeMoveType.Append)
            //    //    throw new UserFriendlyException(L("不能移动顶级节点的前或后面"));

            //    var all = await AsyncQueryableExecuter.ToListAsync(repository.GetAll().OrderByDescending(c => c.Code));
            //    targetChildren = all.Where(c => c.ParentId == null).ToList();
            //    targetIndex = targetChildren.Count;
            //}
            //else
            if (moveType != GeneralTreeMoveType.Append)
            {
                var temp1 = await GetBrotherWithOffspringAsync(target);
                targetParent = temp1.Item1;
                targetChildren = temp1.Item2;
                targetIndex = targetChildren.IndexOf(target) + (moveType == GeneralTreeMoveType.Front ? 0 : 1);
            }
            else
            {
                targetParent = target;
                if (target != null)
                {
                    targetChildren = await GetFlattenOffspringAsync(target.Code);
                    targetChildren = targetChildren.Where(c => c.ParentId == target.Id).ToList();
                }
                else
                {
                    targetChildren = await GetFlattenOffspringAsync();
                    targetChildren = targetChildren.Where(c => c.ParentId ==null).ToList();
                }
                targetIndex = targetChildren.Count;
            }
            return await MoveAsync(source, targetParent, targetChildren, targetIndex);
        }


        async Task<TEntity> MoveAsync(long sourceId, TEntity target, IList<TEntity> targetList, int targetIndex)
        {
            var source = await repository.GetAsync(sourceId);
            return await MoveAsync(source, target, targetList, targetIndex);
        }

        /*
         * 00001
         *      00001.00001
         *      (目标位置)  后续节点及其后代节点均重新计算code
         *      00001.00002
         *
         * 00002
         *      00002.00001
         *      00002.00002    源位置   后代和后续节点及其后代节点都重新计算code
         *            00002.00001.00001
         *      00002.00003
         */
        /// <summary>
        /// 移动节点
        /// </summary>
        /// <param name="source">源节点，对应00002.00002</param>
        /// <param name="target">目标位置的父节点，对应00001</param>
        /// <param name="targetList">目标节点的子节点集合，对应0001.00001、00001.00002</param>
        /// <param name="targetIndex">移动后所处目标的目标位置的索引，对应1</param>
        /// <returns></returns>
        async Task<TEntity> MoveAsync(TEntity source, TEntity target, IList<TEntity> targetList, int targetIndex)
        {
            //如果调用方让节点移动到原来的位置，这种情况暂时没处理

            var sourceParentCode = source.GetParentCode();

            IList<TEntity> brotherList;
            bool betweenBrother = false;//是否本身就是在同级节点下移动

            //1、准备源节点的兄弟节点列表
            if ((target == null && !source.ParentId.HasValue) || target != null && source.ParentId.Equals(target.Id))
            {
                brotherList = targetList;
                betweenBrother = true;
            }
            else
            {
                var temp = await GetBrotherWithOffspringAsync(source);
                brotherList = temp.Item2;
            }

            //2、从源列表中移除源节点
            // var tempSource = brotherList.Single(c => c.Id.Equals(source.Id));
            var souteceIndex = brotherList.IndexOf(source);//source和brotherList集合中存在的那个对象也许不是同一个引用，在EF中有可能是
            brotherList.RemoveAt(souteceIndex);
            if (source.Parent != null)
                source.Parent.Children = brotherList;

            //3、将源节点放到目标位置
            if (betweenBrother && targetIndex > souteceIndex)
                targetIndex--;

            if (targetIndex < 0)
                targetIndex = 0;
            if (targetIndex > targetList.Count)
                targetIndex = targetList.Count;

            targetList.Insert(targetIndex, source);
            if (target != null)
                target.Children = targetList;

            //5、设置目标的父节点
            if (target == null)
            {
                source.ParentId = null;
                source.Parent = null;
            }
            else
            {
                source.ParentId = target.Id;
                source.Parent = target;
            }

            //6、重置相关节点code
            ResetCode(sourceParentCode, brotherList, souteceIndex);
            var taregetCode = target == null ? "" : target.Code;
            ResetCode(taregetCode, targetList, targetIndex);
            //source.Parent = target;//千万不要在重新生成code前设置Parent，因为后面递归生成code
            //if (betweenBrother)
            //{
            //    var tempIndex = targetIndex > souteceIndex ? souteceIndex : targetIndex;
            //    ResetCode(taregetCode, targetList, tempIndex);
            //}
            //else
            //{
            //    ResetCode(sourceParentCode, brotherList, souteceIndex);
            //    ResetCode(taregetCode, targetList, targetIndex);
            //}

            return source;
        }
        async Task<Tuple<TEntity, IList<TEntity>>> GetBrotherWithOffspringAsync(long id)
        {
            return await GetBrotherWithOffspringAsync(await repository.GetAsync(id));

        }
        /// <summary>
        /// 获取指定节点的兄弟节点、父节点
        /// 均包含其后代节点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Item1父节点，Item2兄弟节点</returns>
        async Task<Tuple<TEntity, IList<TEntity>>> GetBrotherWithOffspringAsync(TEntity entity)
        {
            var parentCode = entity.GetParentCode();

            TEntity parent = null;
            IList<TEntity> children;

            if (!parentCode.IsNullOrWhiteSpace())
            {
                children = await GetFlattenOffspringAsync(parentCode);
                parent = children[0];
            }
            else
            {
                //本来也可以用上面的StartsWith，但是直接getAll性能更好
                children = await GetFlattenOffspringAsync();
            }
            children = children.Where(c => c.ParentId.Equals(entity.ParentId)).ToList();
            return new Tuple<TEntity, IList<TEntity>>(parent, children);
        }
        /// <summary>
        /// 根据code获取所有后代节点，并以平铺结构的集合返回
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> GetFlattenOffspringAsync(string code = null)
        {
            var query = repository.GetAll();
            if (!code.IsNullOrWhiteSpace())
                query = query.Where(c => c.Code.StartsWith(code));
            query = query.OrderBy(c => c.Code);
            return await AsyncQueryableExecuter.ToListAsync(query);
        }
        /// <summary>
        /// 递归重新设置code
        /// </summary>
        /// <param name="parentCode">父节点code</param>
        /// <param name="children">子节点集合</param>
        /// <param name="startIndex">从children中指定索引节点开始（跳过前面的）</param>
        public static void ResetCode(string parentCode, IList<TEntity> children, int startIndex = 0)
        {
            for (int i = startIndex; i < children.Count; i++)
            {
                var item = children[i];
                item.Code = BuildCode(parentCode, i);
                if (item.Children != null)
                    ResetCode(item.Code, item.Children);
            }
        }
        /// <summary>
        /// 生成新的code
        /// </summary>
        /// <param name="parentCode"></param>
        /// <param name="lasttCode"></param>
        /// <returns></returns>
        public static string BuildCode(string parentCode, string lasttCode)
        {
            if (parentCode.IsNullOrEmpty())
            {
                if (lasttCode.IsNullOrEmpty())
                    return "1".PadLeft(GeneralTreeEntity.CodeUnitLength, '0');
                else
                {
                    var lastBlock1 = lasttCode.Split('.').Last();
                    var temp1 = Convert.ToInt32(lastBlock1) + 1;
                    return temp1.ToString().PadLeft(GeneralTreeEntity.CodeUnitLength, '0');
                }
            }
            else
            {
                if (lasttCode.IsNullOrEmpty())
                    return parentCode + "." + "1".PadLeft(GeneralTreeEntity.CodeUnitLength, '0');
                else
                {
                    var lastBlock = lasttCode.Split('.').Last();
                    var temp = Convert.ToInt32(lastBlock) + 1;
                    return parentCode + "." + temp.ToString().PadLeft(GeneralTreeEntity.CodeUnitLength, '0');
                }
            }
        }
        /// <summary>
        /// 生成新的code
        /// </summary>
        /// <param name="parentCode"></param>
        /// <param name="lasttCode"></param>
        /// <returns></returns>
        public static string BuildCode(string parentCode, int lasttCode)
        {
            return BuildCode(parentCode, lasttCode.ToString());
        }
        //protected class LastOrParent
        //{
        //    public long Id { get; set; }
        //    public string Code { get; set; }
        //    public long? ParentId { get; set; }
        //    public string ParentCode { get; set; }
        //}
        ///// <summary>
        ///// 根据父级id找到最后一个子类，若没找到则返回父类
        ///// 由于父类实体ParentId Id是泛型，在EF的where条件里行不通，所以这个功能延迟到子类去处理
        ///// </summary>
        ///// <param name="parentId"></param>
        ///// <returns></returns>
        //protected abstract Task<LastOrParent> GetLastOrParent(long? parentId);
        ////{
        ////    //var sf = new Nullable<long>(parentId);
        ////    var query = repository.GetAll()
        ////    .Where(c => (c.ParentId.Equals(parentId) || c.Id.Equals(parentId)))
        ////       .OrderByDescending(c => c.Code)
        ////       //经过7小时的折腾，最后发现不能用匿名类
        ////       .Select(c => new LastOrParent { ParentId = c.ParentId, ParentCode = c.Parent.Code, ChildCode = c.Code });

        ////    return await AsyncQueryableExecuter.FirstOrDefaultAsync(query);
        ////}
        /////public virtual async Task<IList<TEntity>> GetByParentId(long id, bool loadProgeny = false)
        //{ 

        //}

        //protected virtual async Task<string> BuildCode(long? parentId)
        //{
        //    var q = await GetLastOrParent(parentId);
        //    if (q == null)
        //        return BuildCode("", "");
        //    else if (parentId.Equals(q.Id)) //拿到的父节点
        //        return BuildCodeByLastCode(q.Code, "");
        //    else
        //        return BuildCodeByLastCode(q.ParentCode, q.Code);
        //}
        //protected async Task<string> BuildCode(string parentCode)
        //{
        //    if (parentCode.IsNullOrWhiteSpace())
        //        return BuildCodeByLastCode("", "");

        //    var query = repository.GetAll()
        //        .Where(c => c.Code.StartsWith(parentCode))
        //        .OrderByDescending(c => c.Code)
        //        .Select(c => c.Code);

        //    var q = await AsyncQueryableExecuter.FirstOrDefaultAsync(query);
        //    if (q == parentCode)
        //        q = "";

        //    return BuildCodeByLastCode(parentCode, q);
        //}
    }
    /// <summary>
    /// 通用字典领域服务
    /// </summary>
    public class GeneralTreeManager : GeneralTreeManager<GeneralTreeEntity>
    {
        public GeneralTreeManager(IRepository<GeneralTreeEntity, long> repository) : base(repository)
        {
        }
    }
}
