using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.DAL
{
    public class _BaseNumericNamedCodedDescModel : _BaseNumericNamedCodedModel, IDescription
    {
        #region Fields
        public string Description { get; set; }

        #endregion
    }
}
