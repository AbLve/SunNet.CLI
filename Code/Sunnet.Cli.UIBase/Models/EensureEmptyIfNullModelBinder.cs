using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.UIBase.Models
{
    public class EensureEmptyIfNullModelBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            var propertyType = propertyDescriptor.PropertyType;
            var obj = base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
            var eein = propertyDescriptor.Attributes.OfType<EensureEmptyIfNullAttribute>().FirstOrDefault();
            if (obj == null && eein != null)
            {
                if (propertyType == typeof(string))
                    return string.Empty;
                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    return PostFormHelper.DefaultMinDatetime;
            }
            if (obj == null &&
                (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?)))
            {
                return PostFormHelper.DefaultMinDatetime;
            }
            if (obj != null && (propertyType == typeof(string) || propertyType == typeof(String)))
            {
                return obj.ToString().Trim();
            }
            return obj;
        }
    }
}