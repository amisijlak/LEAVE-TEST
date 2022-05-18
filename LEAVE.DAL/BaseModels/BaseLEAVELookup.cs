using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.DAL
{
    public abstract class BaseLEAVELookup : _BaseNumericNamedCodedModel, IActivatable
    {
        #region Fields
        public bool IsActive { get; set; }

        #endregion
    }

    public abstract class BaseLEAVELookupWithDesc : BaseLEAVELookup, IDescription
    {
        #region Fields
        public string Description { get; set; }

        #endregion
    }
}
