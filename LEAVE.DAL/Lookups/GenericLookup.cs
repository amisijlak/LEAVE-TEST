using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LEAVE.DAL.Lookups
{
    [Table("GenericLookups", Schema = LEAVESchemas.LOOKUPS)]
    public class GenericLookup : _BaseNumericNamedCodedDescModel
    {
        #region Fields

        public GenericLookupType LookupType { get; set; }

        #endregion
    }

    public enum GenericLookupType
    {
        Branch = 1,
        Department = 2,
        Position = 3,
        LeaveType = 4,
    }
}
