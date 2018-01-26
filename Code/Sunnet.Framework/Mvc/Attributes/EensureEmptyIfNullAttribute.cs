using System;

namespace Sunnet.Framework.Mvc
{
    /// <summary>
    /// 在允许为空的属性上增加该标识, 如果视图里面该属性未赋值, ModelBinder在处理时会为该属性赋值String.Empty
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EensureEmptyIfNullAttribute : Attribute
    {
        public EensureEmptyIfNullAttribute()
        {

        }
    }
}