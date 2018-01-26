using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/22 20:41:30
 * Description:		Please input class summary
 * Version History:	Created,2014/8/22 20:41:30
 * 
 * 
 **************************************************************************/
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Sunnet.Framework.Helpers;

namespace Sunnet.Framework.Extensions
{
    public static class HtmlSelectExtensions
    {
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, string defaultText, string defaultValue)
        {
            return HtmlSelectExtensions.EnumDropDownListFor<TModel, TEnum>(htmlHelper, expression, defaultText, defaultValue, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, string defaultText, string defaultValue, object htmlAttributes)
        {
            return HtmlSelectExtensions.EnumDropDownListFor<TModel, TEnum>(htmlHelper, expression, defaultText, defaultValue, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, string defaultText, string defaultValue, IDictionary<string, object> htmlAttributes)
        {
            //ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            //IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            IEnumerable<SelectListItem> items = Enum.Parse(typeof(TEnum), "0")
                .CastTo<Enum>()
                .ToSelectList(true, new SelectOptions(true, defaultValue, defaultText));
            return htmlHelper.DropDownListFor(
                expression,
                items, htmlAttributes
                );
        }


        /// <summary>
        /// Convert to select item list.
        /// </summary>
        /// <param name="sources">The sources.</param>
        /// <param name="defaultText">The default text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectList(this IEnumerable<SelectItemModel> sources,
            string defaultText = "", string defaultValue = "")
        {
            var list =
                sources.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.ID.ToString(),
                    Selected = x.Selected
                }).ToList();
            if (!string.IsNullOrEmpty(defaultText))
            {
                list.ForEach(x => x.Selected = false);
                list.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = defaultText,
                    Value = defaultValue
                });
            }
            return list;
        }


        /// <summary>
        /// Conver A Bool Type to A List<SelectListItem>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultText"></param>
        /// <param name="defaultValue"></param>
        /// <param name="yesText"></param>
        /// <param name="yesValue"></param>
        /// <param name="yesSelected"></param>
        /// <param name="noText"></param>
        /// <param name="noValue"></param>
        /// <param name="noSelected"></param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectList(this bool source,
            string defaultText = "", string defaultValue = "",
            string yesText = "Yes", string yesValue = "1", bool yesSelected = true,
            string noText = "No", string noValue = "0", bool noSelected = false
            )
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Selected = yesSelected, Text = yesText, Value = yesValue });
            list.Add(new SelectListItem() { Selected = noSelected, Text = noText, Value = noValue });
            if (!string.IsNullOrEmpty(defaultText))
            {
                list.ForEach(x => x.Selected = false);
                list.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = defaultText,
                    Value = defaultValue
                });
            }
            return list;
        }
    }
}
