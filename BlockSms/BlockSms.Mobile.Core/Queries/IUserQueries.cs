using BlockSms.Core.Queries;
using BlockSms.Core.Domain;
using System.Threading.Tasks;

namespace BlockSms.Mobile.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserQueries : IBaseQueries<User>
    {
        /// <summary>
        /// 手机号查用户
        /// </summary>
        Task<User> GetModelAsync(string mobile);
    }
}
