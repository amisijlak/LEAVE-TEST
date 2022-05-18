using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LEAVE.DAL
{
    public abstract class _BaseNumericModel : INumericPrimaryKey
    {
        #region Fields

        [Key]
        public long Id { get; set; }

        #endregion
    }
}
