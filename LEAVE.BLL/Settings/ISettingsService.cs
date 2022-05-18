using LEAVE.DAL;
using LEAVE.DAL.Lookups;
using LEAVE.DAL.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEAVE.BLL.Settings
{
    public interface ISettingsService
    {
        bool SaveCodedModel<T>(T model, out string errorMessage) where T : class, ICodedModel, INumericPrimaryKey;
        bool DeleteModel<T>(long model, out string errorMessage) where T : class, INumericPrimaryKey;
    }
}
