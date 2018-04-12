using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using Dapper;

namespace PPP2.Data
{
    public delegate void DelSql(string sql);
    public interface IDbContext : IDisposable
    {
        IDbContext InNewTransactionalContext();
        IDbContext InReadContext();
        IDbContext InTransactionalContext();
        void Complete();
        IEnumerable<T> Select<T>(string sql);
        int Execute(string sql, object data = null, CommandType commandType = CommandType.Text);
        IEnumerable<T> ExecProcWithResults<T>(string procName, DynamicParameters param);
        event DelSql OnSqlPrepared;
    }
    public static class DBHelper
    {
        public static string DbSafe(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            return str.Replace("'", "''");
        }
    }
    public static class DbContextProvider
    {
        public static IDbContext dbContext;
        public static string ConnectionString { get; set; }
        public static void SetDbContext(IDbContext dbContext)
        {
            DbContextProvider.dbContext = dbContext;
        }
        public static IDbContext GetDbContext()
        {
            return DbContextProvider.dbContext ?? new Db(ConnectionString);
        }
        public static IDbContext GetDbContext(string connectionString)
        {
            return new Db(connectionString);
        }
    }
    internal class Db : IDbContext
    {
        string ConnStr = string.Empty;
        SqlConnection connection;
        TransactionScope transactionScope;
        public Db(string connectionString)
        {
            this.ConnStr = connectionString;
        }
        public IDbContext InReadContext()
        {
            if (connection == null)
            {
                connection = new SqlConnection(ConnStr);
                connection.Open();
            }
            return this;
        }
        public IDbContext InNewTransactionalContext()
        {
            transactionScope = new TransactionScope(TransactionScopeOption.Suppress);
            if (connection == null)
            {
                connection = new SqlConnection(ConnStr);
                connection.Open();
            }
            return this;
        }
        public IDbContext InTransactionalContext()
        {
            if (Transaction.Current == null)
            {
                transactionScope = new TransactionScope();
                if (connection == null)
                {
                    connection = new SqlConnection(ConnStr);
                    connection.Open();
                }
                if (Transaction.Current != null)
                {
                    connection.EnlistTransaction(Transaction.Current);
                }
            }
            else
            {
                //transactionScope = new TransactionScope(Transaction.Current);
                if (connection == null)
                {
                    connection = new SqlConnection(ConnStr);
                    connection.Open();
                }
                if (Transaction.Current != null)
                {
                    connection.EnlistTransaction(Transaction.Current);
                }
            }
            return this;
        }
        public void Complete()
        {
            if (transactionScope != null)
                transactionScope.Complete();
        }
        public void Dispose()
        {
            if (connection != null)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                connection.Dispose();
            }
            if (transactionScope != null)
            {
                transactionScope.Dispose();
            }
        }
        public IEnumerable<T> Select<T>(string sql)
        {
            if (null != OnSqlPrepared)
            {
                OnSqlPrepared(sql);
            }
            var result = connection.Query<T>(sql);
            return result;
        }
        public int Execute(string sql, object data = null, CommandType commandType = CommandType.Text)
        {
            if (null != OnSqlPrepared)
            {
                OnSqlPrepared(sql);
            }
            return connection.Execute(sql, data, commandType: commandType);
        }
        public IEnumerable<T> ExecProcWithResults<T>(string procName, DynamicParameters param)
        {
            return connection.Query<T>(procName, param, commandType: CommandType.StoredProcedure);
        }
        public event DelSql OnSqlPrepared;
    }
}