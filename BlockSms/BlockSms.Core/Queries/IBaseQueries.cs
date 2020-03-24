using BlockSms.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockSms.Core.Queries
{
    public interface IBaseQueries<T> where T : IEntity
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        Task<IEnumerable<T>> GetListAsync(int? deptId);
        /// <summary>
        /// 分页查询列表
        /// </summary>
        Task<IEnumerable<T>> GetPageAsync(int? deptId, int page = 1, int pageSize = 10);

    }
}
