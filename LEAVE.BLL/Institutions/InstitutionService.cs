using LEAVE.BLL.Data;
using LEAVE.BLL.Helpers;
using LEAVE.BLL.Security;
using LEAVE.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LEAVE.BLL.Institutions
{
    public class InstitutionService : BaseService, IInstitutionService
    {
        public InstitutionService(IDbRepository repository, ISessionService sessionService)
            : base(repository, sessionService)
        {
        }

        public IQueryable<Institution> GetUserInstitutions()
        {
            var query = _repository.Set<Institution>().Select(r => r);
            return query;
        }
    }
}
