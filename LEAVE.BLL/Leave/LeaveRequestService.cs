using LEAVE;
using LEAVE.BLL.Data;
using LEAVE.BLL.Security;
using LEAVE.DAL.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LEAVE.BLL.Leave
{
    public class LeaveRequestService : BaseService, ILeaveRequestService
    {
        public LeaveRequestService(IDbRepository repository, ISessionService sessionService)
            : base(repository, sessionService)
        {
        }

        public IEnumerable<LeaveRequest> GetLeaveRequests()
        {
            return _repository.Set<LeaveRequest>().Include(r => r.Employee).Include(r => r.Employee.Department)
                .Include(r => r.LeaveType);
        }


        public (bool, string) ValidateLeaveRequest(LeaveRequest model)
        {
            string ErrorMessage = "";
            try
            {
                var LeaveList = GetLeaveRequests().Where(r=>r.Id != model.Id);
                var DepartmentId = _repository.Set<Employee>().Where(r => r.Id == model.EmployeeId).SingleOrDefault().DepartmentId;

                if (model.StartDate >= model.EndDate)
                {
                    throw new Exception($"The StartDate {model.StartDate.FormatForDisplay()} CAN NOT be greater than the end date {model.EndDate.FormatForDisplay()}!");
                }

                var MyLeaveRequests = LeaveList.Where(r => r.EmployeeId == model.EmployeeId);

                if (MyLeaveRequests.Where(r => r.EmployeeId == model.EmployeeId).Any(r => r.EndDate >= model.StartDate))
                {
                    throw new Exception("The dates you have selected ovalap with one of your previous requests!");
                }

                if (LeaveList.Any(r => r.Employee.DepartmentId == DepartmentId && r.EndDate >= model.StartDate))
                {
                    throw new Exception("The dates you have selected ovalap with onether employee in your department!");
                }

                var myLastRequestEndDate = MyLeaveRequests.Max(r => r.EndDate);

                //Here we are just considering 30 days being a months , we did not want to calculate the month based on number of working days
                //, since the requirement did not specify that 
                if (myLastRequestEndDate.HasValue)
                {
                    if (GetNumberOfDays(model.StartDate.Value, myLastRequestEndDate.Value) < 30)
                    {
                        throw new Exception($"You can ONLY qualify for another leave after 30 days from your previous leave, Which ended {myLastRequestEndDate.FormatForDisplay()}!");
                    }
                }
            }catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return (ErrorMessage.IsNullOrEmpty(), ErrorMessage);
        }
       
        public double GetNumberOfDays(DateTime date1, DateTime date2)
        {
            return (date1 - date2).TotalDays;
        }

        public (bool, string) SaveLeaveRequest(LeaveRequest model)
        {
            try
            {
                var validationResult = ValidateLeaveRequest(model);
                if (validationResult.Item1)
                {
                    var dbModel = _repository.Set<LeaveRequest>().Where(r => r.Id == model.Id).SingleOrDefault();
                    if (dbModel == null)
                    {
                        _repository.Set<LeaveRequest>().Add(model);
                    }
                    else
                    {
                        _repository.UpdateDatabaseModel<LeaveRequest>(dbModel, model);
                    }
                    _repository.SaveChanges();
                    return (true, "");
                }
                return validationResult;
            }
            catch (Exception e)
            {
                return (false, e.ExtractInnerExceptionMessage());
            }
        }
    }
}
