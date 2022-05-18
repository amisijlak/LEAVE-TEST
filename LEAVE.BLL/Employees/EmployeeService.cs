using LEAVE.BLL.Data;
using LEAVE.BLL.Helpers;
using LEAVE.BLL.Security;
using LEAVE.DAL.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEAVE.BLL.Employees
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        public EmployeeService(IDbRepository repository, ISessionService sessionService) : base(repository, sessionService) { }

        public IEnumerable<Employee> GetEmployees()
        {
            return _repository.Set<Employee>().Include(r => r.Department).Include(r => r.Position).Include(r => r.User);
        }

        public (bool, string) SaveEmployee(Employee model)
        {
            try
            {
                var dbModel = _repository.Set<Employee>().Where(r => r.Id == model.Id).SingleOrDefault();
                if (dbModel == null) //New Employee
                {
                    _repository.Set<Employee>().Add(model);
                }
                else
                {
                    _repository.UpdateDatabaseModel<Employee>(dbModel, model);
                }
                _repository.SaveChanges();
                return (true, "");
            }
            catch (Exception e)
            {
                return (false, e.ExtractInnerExceptionMessage());
            }
        }
    }
}
