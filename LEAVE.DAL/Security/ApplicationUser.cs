using LEAVE.DAL.Entities;
using LEAVE.DAL.Lookups;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LEAVE.DAL.Security
{
    public class ApplicationUser : IdentityUser<string>, IPersonName, IUserName, ICodedModel, IPrimaryKeyEnabled<string>
    {
        #region Fields

        [MaxLength(CONSTANTS.CODE_FIELD_LENGTH), RegularExpression(CONSTANTS.CODE_FIELD_REGEX, ErrorMessage = CONSTANTS.CODE_FIELD_REGEX_ERROR_MESSAGE), Required]
        public string Code { get; set; }
        [Required, MaxLength(255)]
        public string FirstName { get; set; }
        [Required, MaxLength(255)]
        public string LastName { get; set; }
        [MaxLength(255)]
        public string OtherName { get; set; }
        [ForeignKey("Institution"), Display(Name = "Institution")]
        public long? InstitutionId { get; set; }
        [ForeignKey("Branch"), Display(Name = "Branch")]
        public long? BranchId { get; set; }
        public UserType UserType { get; set; }

        #endregion

        #region Extra Logic

        /// <summary>
        /// NOT MAPPED
        /// </summary>
        [NotMapped]
        public string ErrorMessage { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Institution Institution { get; set; }
        public virtual GenericLookup Branch { get; set; }

        #endregion

        public bool IsLockedOut()
        {
            return LockoutEnd == null ? false : LockoutEnd > DateTime.Now;
        }
    }

    public enum UserType
    {
        Bank_User = 0,
        Interswitch_User = 1,
        Corporate_User = 2,
    }
}
