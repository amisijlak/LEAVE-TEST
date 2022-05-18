using LEAVE.BLL.Data;
using LEAVE.BLL.Helpers;
using LEAVE.BLL.Security;
using LEAVE.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LEAVE.BLL
{
    public abstract class BaseService
    {
        protected readonly IDbRepository _repository;
        protected readonly ISessionService _sessionService;

        public BaseService(IDbRepository repository, ISessionService sessionService)
        {
            this._repository = repository;
            this._sessionService = sessionService;
        }

        protected void _SaveCodedModel<T>(T Model) where T : class, INumericPrimaryKey, ICodedModel
        {
            if (_repository.Set<T>().Any(r => r.Id != Model.Id && r.Code == Model.Code))
            {
                throw new Exception("Another record with the same Code already exists!");
            }

            _SaveModel<T, long>(Model);
        }

        protected void _SaveModel<T, O>(T Model) where T : class, IPrimaryKeyEnabled<O>
        {
            List<string> logXML = new List<string>();

            var dbSet = _repository.Set<T>();

            var dbModel = dbSet.Find(Model.Id);

            var logMessage = dbModel == null ? "Created" : "Updated";

            if (dbModel == null)//New
            {
                dbSet.Add(Model);
            }
            else//existing
            {
                _repository.UpdateDatabaseModel(dbModel, Model);
            }

            _repository.SaveChanges();
        }

        protected void _DeleteModel<T, O>(O Id) where T : class, IPrimaryKeyEnabled<O>
        {
            var dbSet = _repository.Set<T>();

            var dbModel = dbSet.Find(Id);
            if (dbModel == null) throw new Exception("Record Not Found!");

            dbSet.Remove(dbModel);

            _repository.SaveChanges();
        }
    }
}
