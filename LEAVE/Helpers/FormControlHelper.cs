using LEAVE.DAL;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LEAVE.DAL
{
    public static class FormControlHelper
    {
        /// <summary>
        /// Renders a Textbox in a div with a label in a form-group div with a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="HtmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlContent TextBoxWithLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression, object HtmlAttributes = null, string Format = null)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithLabel", Html.LabelFor(Expression), Html.TextBoxFor(Expression, Format, HtmlAttributes)
                , Html.ValidationMessageFor(Expression));
        }


        /// <summary>
        /// Renders a Textbox in a div with a label in a div with the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="HtmlAttributes"></param>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static IHtmlContent TextBoxWithoutLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression, object HtmlAttributes = null, string Format = null)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithoutLabel", null, Html.TextBoxFor(Expression, Format, HtmlAttributes)
                , Html.ValidationMessageFor(Expression));
        }

        /// <summary>
        /// Renders a Dropdown List with a label in a form-group div with a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="Expression"></param>
        /// <param name="SelectList"></param>
        /// <param name="HtmlAttributes"></param>
        /// <param name="OptionalLabel"></param>
        /// <returns></returns>
        public static IHtmlContent DropDownListWithLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression
            , SelectList SelectList, string OptionalLabel = null, object HtmlAttributes = null)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithLabel", Html.LabelFor(Expression),
                Html.DropDownListFor(Expression, SelectList, OptionalLabel, HtmlAttributes), Html.ValidationMessageFor(Expression));
        }

        /// <summary>
        /// Renders a Dropdown List with a label in a div with the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="Expression"></param>
        /// <param name="SelectList"></param>
        /// <param name="HtmlAttributes"></param>
        /// <param name="OptionalLabel"></param>
        /// <returns></returns>
        public static IHtmlContent DropDownListWithoutLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression
            , SelectList SelectList, string OptionalLabel = null, object HtmlAttributes = null)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithoutLabel", null,
                Html.DropDownListFor(Expression, SelectList, OptionalLabel, HtmlAttributes), Html.ValidationMessageFor(Expression));
        }

        /// <summary>
        /// Renders a TextArea in a form-group div with a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="HtmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlContent TextAreaWithLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression, object HtmlAttributes = null)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithLabel", Html.LabelFor(Expression), Html.TextAreaFor(Expression, HtmlAttributes)
                , Html.ValidationMessageFor(Expression));
        }

        /// <summary>
        /// Renders a TextArea in a div with the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="HtmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlContent TextAreaWithoutLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression, object HtmlAttributes = null)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithoutLabel", null, Html.TextAreaFor(Expression, HtmlAttributes)
                , Html.ValidationMessageFor(Expression));
        }
        /// <summary>
        /// Renders a Password in a form-group div with a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="HtmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlContent PasswordWithLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression, object HtmlAttributes = null)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithLabel", Html.LabelFor(Expression), Html.PasswordFor(Expression, HtmlAttributes)
                , Html.ValidationMessageFor(Expression));
        }
        /// <summary>
        /// Renders a Date Picker in a form-group div with a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="AllowFutureDates"></param>
        /// <returns></returns>
        public static IHtmlContent DatePickerWithLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression, bool AllowFutureDates = false, bool ShowTime = false, bool IsRequired = false
            , bool ShowTimeOnly = false, bool NoPastDates = false)
        {
            return Html._datepickerFor(Expression, Html.LabelFor(Expression), AllowFutureDates, ShowTime, IsRequired, ShowTimeOnly, NoPastDates);
        }

        /// <summary>
        /// Renders a Date Picker in a form-group div with a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="AllowFutureDates"></param>
        /// <returns></returns>
        public static IHtmlContent DatePickerWithoutLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression
            , bool AllowFutureDates = false, bool ShowTime = false, bool IsRequired = false, bool ShowTimeOnly = false, bool NoPastDates = false)
        {
            return Html._datepickerFor(Expression, null, AllowFutureDates, ShowTime, IsRequired, ShowTimeOnly, NoPastDates);
        }

        public const string DATE_FORMAT = "{0:yyyy-MM-dd}";
        public const string DATE_AND_TIME_FORMAT = "{0:yyyy-MM-dd HH:mm}";

        private static IHtmlContent _datepickerFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression
            , object Label, bool AllowFutureDates = false, bool ShowTime = false, bool IsRequired = false, bool ShowTimeOnly = false
            , bool NoPastDates = false)
        {
            var cssClass = "datepicker " + (NoPastDates ? "no-past" : (AllowFutureDates ? "future" : "present")) + (ShowTime ? " show-time" : "")
                + (ShowTimeOnly ? " time-only" : "");

            string format = ShowTimeOnly ? "{0:HH:mm}" : (ShowTime ? DATE_AND_TIME_FORMAT : DATE_FORMAT);
            object htmlAttributes = IsRequired ? (object)new { @class = cssClass, required = "required" } : new { @class = cssClass };

            return RenderPartial(Html, $"Controls/{(Label == null ? "_PartialControlWithoutLabel" : "_PartialControlWithLabel")}", Label
                , Html.TextBoxFor(Expression, format, htmlAttributes)
                , Html.ValidationMessageFor(Expression));
        }

        /// <summary>
        /// Renders a Date Picker for the future starting today in a form-group div with a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="AllowFutureDates"></param>
        /// <returns></returns>
        public static IHtmlContent FutureDatePickerWithLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression)
        {
            return Html.DatePickerWithLabelFor(Expression, NoPastDates: true);
        }

        /// <summary>
        /// Renders a Date Picker for the future starting today in a form-group div with a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="AllowFutureDates"></param>
        /// <returns></returns>
        public static IHtmlContent FutureDatePickerWithOutLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression)
        {
            return Html.DatePickerWithoutLabelFor(Expression, NoPastDates: true);
        }

        /// <summary>
        /// Renders a Date Picker in a form-group div without a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="AllowFutureDates"></param>
        /// <returns></returns>
        public static IHtmlContent CustomDatePickerWithoutLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression, bool AllowFutureDates = false)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithoutLabel", null
                , Html.TextBoxFor(Expression, "{0:yyyy-MM-dd}", new { @class = "datepicker customstartDate", @readonly = "readonly" })
                , Html.ValidationMessageFor(Expression));
        }

        /// <summary>
        /// Renders a Password in a form-group div with a label, the control and validation message
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Html"></param>
        /// <param name="Label"></param>
        /// <param name="HtmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlContent PasswordWithLabel<TModel>(this IHtmlHelper<TModel> Html, string Label, string ControlId, object HtmlAttributes = null)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithLabel", Html.Label(Label), Html.Password(ControlId, null, HtmlAttributes)
                , Html.ValidationMessage(ControlId));
        }

        /// <summary>
        /// Displays a value in a div with a label
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="HtmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlContent DisplayWithLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> Html, Expression<Func<TModel, TProperty>> Expression)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithLabel", Html.LabelFor(Expression), Html.DisplayFor(Expression)
                , null);
        }

        /// <summary>
        /// Displays a value in a div with a label
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="HtmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlContent DisplayWithLabel<TModel>(this IHtmlHelper<TModel> Html, string Label, string Value)
        {
            return RenderPartial(Html, "Controls/_PartialControlWithLabel", Html.Label(Label), Value
                , null);
        }

        private static IHtmlContent RenderPartial(IHtmlHelper Html, string PartialUrl, object Label, object Control, object Validation)
        {
            Html.ViewData["Label"] = Label;
            Html.ViewData["Control"] = Control;
            Html.ViewData["Validation"] = Validation;

            return Html.Partial(PartialUrl);
        }

        /// <summary>
        /// Renders a Hidden Fields especially useful for Id columns in partials
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Html"></param>
        /// <param name="expression"></param>
        /// <param name="HtmlAttributes"></param>
        /// <returns></returns>
        public static HtmlString HiddenField<TModel>(this IHtmlHelper<TModel> Html, string Name, object Value)
        {
            return (HtmlString)Html.Raw("<input type=\"hidden\" name=\"" + Name + "\" id=\"" + Name + "\" value=\"" + Value + "\" />");
        }

        public static IHtmlContent CodeFieldControlWithLabel<TModel>(this IHtmlHelper<TModel> Html) where TModel : ICodedModel
        {
            return Html.TextBoxWithLabelFor(m => m.Code, new
            {
                @data_val_maxlength = CONSTANTS.CODE_FIELD_LENGTH,
                @required = "required",
                @data_val= "true",
                @data_val_regex_pattern = CONSTANTS.CODE_FIELD_REGEX,
                @data_val_regex = CONSTANTS.CODE_FIELD_REGEX_ERROR_MESSAGE
            });
        }

        public static IHtmlContent NameFieldControlWithLabel<TModel>(this IHtmlHelper<TModel> Html) where TModel : INamedModel
        {
            return Html.TextBoxWithLabelFor(m => m.Name, new { @maxlength = "255", @required = "required" });
        }
    }
}
