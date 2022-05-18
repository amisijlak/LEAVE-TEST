using LEAVE.BLL.Helpers;
using LEAVE.BLL.Security;
using LEAVE.DAL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LEAVE.BLL.Data
{
    public class LEAVERepository : IDbRepository
    {
        private readonly LEAVEContext _database;


        public LEAVERepository(LEAVEContext db)
        {
            this._database = db;
        }

        public void ExecuteInNewTransaction(Action LogicToExecute)
        {
            using (var transaction = _database.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    LogicToExecute();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public DbSet<T> Set<T>() where T : class
        {
            return _database.Set<T>();
        }

        public void UpdateDatabaseModel<T>(T dbItem, T updatedItem) where T : class
        {
            _database.Entry(dbItem).CurrentValues.SetValues(updatedItem);
        }

        public void SaveChanges()
        {
            _database.SaveChanges();
        }

        public void LoadReference<T>(T model, Expression<Func<T, object>> Property) where T : class
        {
            _database.Entry(model).Reference(Property).Load();
        }

        public void LoadCollection<T>(T model, Expression<Func<T, IEnumerable<object>>> CollectionProperty) where T : class
        {
            _database.Entry(model).Collection(CollectionProperty).Load();
        }

        public void ExecuteSqlCommand(string query, params DbParameter[] parameters)
        {
            _database.Database.ExecuteSqlRaw(query, parameters);
        }

        public IEnumerable<T> SqlQuery<T>(string query, Func<DbDataReader, T> ExtractDataFtn, params SqlParameter[] parameters)
        {
            using (var con = new SqlConnection(_database.ConnectionString))
            {
                con.Open();

                var cmd = new SqlCommand(query, con);
                if (parameters.Any()) cmd.Parameters.AddRange(parameters);

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        yield return ExtractDataFtn(reader);
                    }
                }
            }

        }

        public string GetConnectionString()
        {
            return _database?.ConnectionString;
        }

        public void Remove<T>(T model) where T : class
        {
            _database.Remove(model);
        }

        public void SetConnectionTimeout(int timeout)
        {
            _database?.Database.SetCommandTimeout(timeout);
        }
    }
}
