using LEAVE.DAL;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LEAVE
{
    public static class CustomExtensionMethods
    {
        /// <summary>
        /// Splits the SearchTerm using the space character
        /// </summary>
        /// <param name="SearchTerm"></param>
        /// <returns></returns>
        public static string[] GetSearchTerms(this string SearchTerm)
        {
            return SearchTerm?.Split(' ') ?? new string[0];
        }

        #region Partials

        public static IHtmlContent RenderSuccessMessageControl(this IHtmlHelper Html)
        {
            return Html.Partial("Messages/_Success");
        }

        public static IHtmlContent RenderErrorMessageControl(this IHtmlHelper Html)
        {
            return Html.Partial("Messages/_Error");
        }

        public static IHtmlContent RenderSaveButtonsControl(this IHtmlHelper Html, bool renderSaveAndContinueButtonAsWell)
        {
            return Html.Partial("Controls/_SaveButtons", renderSaveAndContinueButtonAsWell);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="ActionName">MVC Action</param>
        /// <param name="saveButtonType"></param>
        /// <returns></returns>
        public static IHtmlContent RenderReloadForm(this IHtmlHelper Html, string ActionName, LEAVESaveButtonType? saveButtonType = null)
        {
            Html.ViewData["_ReloadFormSuffix"] = saveButtonType == null ? "" : "-" + saveButtonType.GetIntValue();
            Html.ViewData["ReloadFormUseAction"] = false;
            return Html.Partial("Controls/_ReloadForm", ActionName);
        }

        public static IHtmlContent RenderReloadFormUsingUrl(this IHtmlHelper Html, string url, LEAVESaveButtonType? saveButtonType = null)
        {
            Html.ViewData["_ReloadFormSuffix"] = saveButtonType == null ? "" : "-" + saveButtonType.GetIntValue();
            Html.ViewData["ReloadFormUseAction"] = true;
            return Html.Partial("Controls/_ReloadForm", url);
        }

        #endregion

        #region Enums

        public static List<T> GetLEAVEEnumValues<T>(this IHtmlHelper Html)
        {
            List<T> enumTypes = new List<T>();

            foreach (var value in Enum.GetValues(typeof(T)))
            {
                enumTypes.Add((T)value);
            }

            return enumTypes;
        }

        public static SelectList GetLEAVEEnumSelectList<T>(this IHtmlHelper Html)
        {
            List<object> enumTypes = new List<object>();

            foreach (Enum value in Enum.GetValues(typeof(T)))
            {
                enumTypes.Add(new { Key = value.GetEnumName(), Value = value.GetIntValue() });
            }

            return new SelectList(enumTypes, "Value", "Key");
        }

        #endregion
                
        #region DatePickers

        public static IHtmlContent DatePickerFor<T, TProperty>(this IHtmlHelper<T> Html, Expression<Func<T, TProperty>> Property)
        {
            return Html.TextBoxFor(Property, "{0:yyyy-MM-dd}", new { @class = "form-control datepicker" });
        }

        #endregion

        /// <summary>
        /// Display as Raw Html i.e. without escaping any characters
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HtmlString Raw(this object value)
        {
            return new HtmlString(value?.ToString());
        }
    }

    public enum LEAVESaveButtonType
    {
        Save = 1,
        Save_And_Continue = 2
    }
}
