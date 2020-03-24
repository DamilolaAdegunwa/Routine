using BlockSms.Core.Queries;
using EPT.Tickets.Self.Domain;
using System.Threading.Tasks;

namespace BlockSms.Mobile.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserQueries : IBaseQueries<User>
    {
        /// <summary>
        /// 查询支付信息
        /// </summary>
        Task<User> GetModelAsync(int deptId, string orderNo);
    }
}
