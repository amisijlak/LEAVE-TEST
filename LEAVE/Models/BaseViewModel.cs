using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LEAVE.DAL
{
    public abstract class BaseViewModel : IViewModel
    {
        public int? CurrentPage { get; set; }
        public string SearchTerm { get; set; }
        public string ErrorMessage { get; set; }
        public int? PageSize { get; set; }
    }
}
