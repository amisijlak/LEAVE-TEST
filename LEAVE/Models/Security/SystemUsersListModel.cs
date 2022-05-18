using LEAVE.BLL.Data;
using LEAVE.DAL.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LEAVE.DAL.Models.Security
{
    public class SystemUsersListModel : IErrorMessage
    {
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public string ErrorMessage { get; set ; }

        public void LoadUsers(IDbRepository repository)
        {
            Users = repository.Set<ApplicationUser>().Select(r => r).Include(r => r.Institution)
                .Include(r => r.Branch).ToList();
        }
    }
}
