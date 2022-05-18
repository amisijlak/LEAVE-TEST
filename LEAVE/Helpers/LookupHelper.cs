using LEAVE.BLL.Data;
using LEAVE.BLL.Security;
using LEAVE.DAL;
using LEAVE.DAL.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LEAVE.DAL
{
    public static class LookupHelper
    {
        #region ICodeMapped Lookups

        private static SelectList ConvertCodedModelToSelectList<T, O>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<O>, ICodedModel, INamedModel
        {
            var data = query.OrderBy(r => r.Code).ToList().Select(r => new { Key = $"{r.Code}: {r.Name}", Value = r.Id }).ToList();

            return new SelectList(data, "Value", "Key");
        }

        public static SelectList ConvertToSelectList<T>(this IEnumerable<T> items,Func<T,object>KeyFtn,Func<T, object> ValueFtn)
        {
            var data = items.Select(r => new { key = KeyFtn(r), value = ValueFtn(r) }).ToList();

            return new SelectList(data, "value", "key");
        }

        public static SelectList ConvertCodedNumericModelToSelectList<T>(this IQueryable<T> query) where T : class, INumericPrimaryKey, ICodedModel, INamedModel
        {
            return ConvertCodedModelToSelectList<T, long>(query);
        }

        public static SelectList ConvertCodedGuidModelToSelectList<T>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<Guid>, ICodedModel, INamedModel
        {
            return ConvertCodedModelToSelectList<T, Guid>(query);
        }

        public static SelectList ConvertCodedStringModelToSelectList<T>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<string>, ICodedModel, INamedModel
        {
            return ConvertCodedModelToSelectList<T, string>(query);
        }

        #endregion

        #region INameMapped Lookups

        private static SelectList ConvertNamedModelToSelectList<T, O>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<O>, INamedModel
        {
            var data = query.OrderBy(r => r.Name).ToList().Select(r => new { Key = r.Name, Value = r.Id }).ToList();

            return new SelectList(data, "Value", "Key");
        }

        public static SelectList ConvertNamedNumericModelToSelectList<T>(this IQueryable<T> query) where T : class, INumericPrimaryKey, INamedModel
        {
            return ConvertNamedModelToSelectList<T, long>(query);
        }

        public static SelectList ConvertNamedGuidModelToSelectList<T>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<Guid>, INamedModel
        {
            return ConvertNamedModelToSelectList<T, Guid>(query);
        }

        public static SelectList ConvertNamedStringModelToSelectList<T>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<string>, INamedModel
        {
            return ConvertNamedModelToSelectList<T, string>(query);
        }

        #endregion

        public static SelectList GetInstitutionUserSelectList(IDbRepository repository, ISessionService sessionService)
        {
            var Users = repository.Set<ApplicationUser>().Select(r => new { Key = r.GetFullName(), Value = r.Id }).ToList(); ;

            return new SelectList(Users, "Value", "Key");
        }
    }
}
