using LEAVE.DAL.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.BLL.Employees
{
    public interface IEmployeeService
    {
        (bool, string) SaveEmployee(Employee model);
        IEnumerable<Employee> GetEmployees();
    }
}
