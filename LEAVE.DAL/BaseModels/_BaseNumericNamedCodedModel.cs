using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LEAVE.DAL
{
    public abstract class _BaseNumericNamedCodedModel : _BaseNumericCodedModel, INamedModel
    {
        #region Fields

        [Required, MaxLength(255)]
        public string Name { get; set; }

        #endregion
    }
}
