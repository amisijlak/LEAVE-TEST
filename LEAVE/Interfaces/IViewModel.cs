using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LEAVE.DAL
{
    /// <summary>
    /// Represents both a view model with core properties for paging and handling errors
    /// </summary>
    public interface IViewModel : IErrorMessage, ISearchTerm
    {
        int? CurrentPage { get; set; }
        int? PageSize { get; set; }

        public static int[] PageSizes => new[] { 15, 30, 50, 70, 100, 150, 200 };
    }
}
