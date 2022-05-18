using LEAVE.DAL.Settings;
using System.Collections.Generic;

namespace LEAVE.BLL.Leave
{
    public interface ILeaveRequestService
    {
        IEnumerable<LeaveRequest> GetLeaveRequests();
        (bool, string) ValidateLeaveRequest(LeaveRequest model);
        (bool, string) SaveLeaveRequest(LeaveRequest model);
    }
}
