﻿using BlockSms.DapperExtension;
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
    public class UserQueries : IUserQueries
    {
        private string _connectionString = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constr"></param>
        public UserQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
            DapperAsyncExtensions.DefaultMapper = typeof(PrefixCoreClassMapper<>);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        public async Task<IEnumerable<User>> GetListAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                IFieldPredicate predicate = null;
                var result = await connection.GetListAsync<User>(predicate);
                return result;
            }
        }
        /// <summary>
        /// 分页查询列表
        /// </summary>
        public async Task<IEnumerable<User>> GetPageAsync(int page = 1, int pageSize = 10)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                IFieldPredicate predicate = null;
                var result = await connection.GetPageAsync<User>(predicate, page: page, resultsPerPage: pageSize);
                return result;
            }
        }
        /// <summary>
        /// 查询支付信息
        /// </summary>
        public async Task<User> GetModelAsync(string mobile)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                pg.Predicates.Add(Predicates.Field<User>(f => f.Phone, Operator.Eq, mobile));
                var result = await connection.GetListAsync<User>(pg);
                if (result.AsList().Count == 0)
                    return null;
                return result.FirstOrDefault();
            }
        }

    }
}
