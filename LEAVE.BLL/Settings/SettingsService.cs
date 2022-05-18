using LEAVE.BLL.Data;
using LEAVE.BLL.Helpers;
using LEAVE.BLL.Security;
using LEAVE.DAL.Lookups;
using LEAVE.DAL.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEAVE.BLL.Settings
{
    public class SettingsService : BaseService, ISettingsService
    {
        public SettingsService(IDbRepository repository, ISessionService sessionService) : base(repository, sessionService) { }

        bool ISettingsService.DeleteModel<T>(long model, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                _repository.ExecuteInNewTransaction(() =>
                {
                    _DeleteModel<T, long>(model);
                });

                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }

        bool ISettingsService.SaveCodedModel<T>(T model, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                _repository.ExecuteInNewTransaction(() =>
                {
                    _SaveCodedModel(model);
                });

                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }
    }
}
