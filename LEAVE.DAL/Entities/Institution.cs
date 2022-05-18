using LEAVE.DAL.Lookups;
using LEAVE.DAL.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LEAVE.DAL.Entities
{
    [Table("Institutions", Schema = LEAVESchemas.ENTITIES)]
    public class Institution : _BaseNumericNamedCodedModel, IActivatable
    {
        #region Fields

        public bool IsActive { get; set; }


        #endregion
    }
}
