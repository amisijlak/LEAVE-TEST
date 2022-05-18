using LEAVE.BLL.Security;
using LEAVE.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEAVE.BLL.Institutions
{
    public interface IInstitutionService
    {
        /// <summary>
        /// Filter Distributors by those accessible to the user.
        /// <para><see cref="CONSTANTS.SUPER_ROLE"/> has access to all.</para>
        /// </summary>
        /// <returns></returns>
        IQueryable<Institution> GetUserInstitutions();
    }
}
