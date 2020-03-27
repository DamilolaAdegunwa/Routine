using BlockSms.Core.Queries;
using BlockSms.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockSms.Mobile.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageQueries : IBaseQueries<Message>
    {
        /// <summary>
        /// 手机号查发送的消息
        /// </summary>
        Task<Message> GetModelAsync(string mobile);
        /// <summary>
        /// 查看发送过的短信
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<IEnumerable<Message>> GetListAsync(string phone);

    }
}
