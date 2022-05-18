using LEAVE.BLL.Helpers;
using LEAVE.BLL.Security;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace LEAVE.BLL.Data
{
    public interface IDbRepository
    {

        /// <summary>
        /// Execute the specified logic in a new Transaction
        /// </summary>
        /// <param name="LogicToExecute"></param>
        void ExecuteInNewTransaction(Action LogicToExecute);

        DbSet<T> Set<T>() where T : class;

        void UpdateDatabaseModel<T>(T dbItem, T updatedItem) where T : class;

        void SaveChanges();

        void LoadReference<T>(T model, Expression<Func<T, object>> Property) where T : class;
        void LoadCollection<T>(T model, Expression<Func<T, IEnumerable<object>>> CollectionProperty) where T : class;

        void ExecuteSqlCommand(string query, params DbParameter[] parameters);
        IEnumerable<T> SqlQuery<T>(string query, Func<DbDataReader, T> ExtractDataFtn, params SqlParameter[] parameters);

        string GetConnectionString();
        void Remove<T>(T model) where T : class;

        void SetConnectionTimeout(int timeout);
    }
}
