using LEAVE.DAL.Lookups;
using LEAVE.DAL.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LEAVE.DAL.Settings
{
    [Table("Employees", Schema = LEAVESchemas.SETTINGS)]
    public class Employee : _BaseNumericCodedModel, IPersonName, IUserMapped
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public string FirstName { get ; set ; }
        public string LastName { get ; set ; }
        public string OtherName { get ; set ; }
        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Date Joined")]
        public DateTime? DateJoined { get; set; }
        [Display(Name = "Level Of Education")]
        public string LevelOfEducation { get; set; }
        [ForeignKey("Department")]
        [Display(Name= "Department")]
        public long? DepartmentId { get; set; }
        [ForeignKey("Position")]
        [Display(Name = "Position")]
        public long? PositionId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual GenericLookup Department { get; set; }
        public virtual GenericLookup Position { get; set; }
    }
}
