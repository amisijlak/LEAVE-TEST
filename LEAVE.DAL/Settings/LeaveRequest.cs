using LEAVE.DAL.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LEAVE.DAL.Settings
{
    [Table("LeaveRequests", Schema = LEAVESchemas.SETTINGS)]
    public class LeaveRequest : _BaseNumericModel
    {
        [ForeignKey("Employee")]
        [Display(Name = "Employee")]
        public long EmployeeId { get; set; }
        [ForeignKey("LeaveType")]
        [Display(Name ="Leave Type")]
        public long LeaveTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual GenericLookup LeaveType { get; set; }
    }
}
