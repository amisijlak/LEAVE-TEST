using LEAVE.BLL.Data;
using LEAVE.BLL.Employees;
using LEAVE.BLL.Leave;
using LEAVE.DAL.Entities;
using LEAVE.DAL.Lookups;
using LEAVE.DAL.Settings;
using System;
using System.Linq;
using X.PagedList;

namespace LEAVE.DAL.Models.Settings
{
    public class SettingsListModel : BaseViewModel
    {
        public IPagedList<Institution> Items { get; set; }
        public IPagedList<GenericLookup> Lookups { get; set; }
        public IPagedList<Employee> Employees { get; set; }
        public IPagedList<LeaveRequest> LeaveRequests { get; set; }

        public object GetRouteValues(long Page)
        {
            return new
            {
                Page,
                PageSize,
                SearchTerm
            };
        }

        public void LoadData(IDbRepository _repository)
        {
            try
            {
                var query = _repository.Set<Institution>().Select(r => r);

                foreach (var term in SearchTerm.GetSearchTerms())
                {
                    query = query.Where(r => r.Name.Contains(term));
                }

                Items = query.ToPagedList(CurrentPage ?? 1, PageSize ?? 20);
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
            }
        }

        public void LoadGenericLookups(IDbRepository _repository)
        {
            try
            {
                var query = _repository.Set<GenericLookup>().Select(r => r);

                foreach (var term in SearchTerm.GetSearchTerms())
                {
                    query = query.Where(r => r.Name.Contains(term));
                }

                Lookups = query.ToPagedList(CurrentPage ?? 1, PageSize ?? 20);
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
            }
        }

        public void LoadEmployees(IEmployeeService employeeService)
        {
            try
            {
                var query = employeeService.GetEmployees();

                foreach (var term in SearchTerm.GetSearchTerms())
                {
                    query = query.Where(r => r.FirstName.Contains(term) || r.LastName.Contains(term) || r.OtherName.Contains(term));
                }

                Employees = query.ToPagedList(CurrentPage ?? 1, PageSize ?? 20);
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
            }
        }
        
        public void LoadELeaveRequests(ILeaveRequestService leaveService)
        {
            try
            {
                var query = leaveService.GetLeaveRequests();

                LeaveRequests = query.ToPagedList(CurrentPage ?? 1, PageSize ?? 20);
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
            }
        }
    }
}
