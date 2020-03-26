using BlockSms.Core;
using BlockSms.DapperExtension;
using BlockSms.DapperExtension.Mapper;
using BlockSms.DapperExtensions;
using Dapper;
using BlockSms.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BlockSms.Mobile.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageQueries : IMessageQueries
    {
        private string _connectionString = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constr"></param>
        public MessageQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
            DapperAsyncExtensions.DefaultMapper = typeof(PrefixCoreClassMapper<>);
        }
        /// <summary>
        /// 通过手机号获取最近发送的消息
        /// </summary>
        public async Task<Message> GetModelAsync(string phone)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                pg.Predicates.Add(Predicates.Field<Message>(f => f.Phone, Operator.Eq, phone));
                var result = await connection.GetListAsync<Message>(pg);
                if (result.AsList().Count == 0)
                    return null;
                return result.OrderByDescending(c => c.SendTime).FirstOrDefault();
            }
        }
        /// <summary>
        /// 查询列表
        /// </summary>
        public async Task<IEnumerable<Message>> GetListAsync(string phone)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                IFieldPredicate predicate = null;
                if (string.IsNullOrEmpty(phone))
                    return null;
                else
                {
                    predicate = Predicates.Field<Message>(f => f.Phone, Operator.Eq, phone);
                    var result = await connection.GetListAsync<Message>(predicate);
                    return result;
                }
            }
        }
        /// <summary>
        /// 分页查询列表
        /// </summary>
        public async Task<IEnumerable<Message>> GetPageAsync(int page = 1, int pageSize = 10)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                IFieldPredicate predicate = null;
                var result = await connection.GetPageAsync<Message>(predicate, page: page, resultsPerPage: pageSize);
                return result;
            }
        }

        public async Task<IEnumerable<Message>> GetListAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.GetListAsync<Message>();
                if (result.AsList().Count == 0)
                    return null;
                return result;
            }
        }
    }
}
