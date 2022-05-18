using LEAVE.BLL.Data;
using System;
using System.Linq;
using X.PagedList;

namespace LEAVE.DAL.Models
{
    public class GenericListViewModel<T> : BaseViewModel where T : class, INamedModel
    {

        public IPagedList<T> Items { get; set; }

        public object GetRouteValues(long Page)
        {
            return new
            {
                Page,
                PageSize,
                SearchTerm
            };
        }

        public void LoadData(IDbRepository _repository, Func<IQueryable<T>, IOrderedQueryable<T>> OrderByClause)
        {
            try
            {
                var query = _repository.Set<T>().Select(r => r);

                foreach (var term in SearchTerm.GetSearchTerms())
                {
                    query = query.Where(r => r.Name.Contains(term));
                }

                Items = OrderByClause(query).ToPagedList(CurrentPage ?? 1, PageSize ?? 20);
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
            }
        }
    }
}
