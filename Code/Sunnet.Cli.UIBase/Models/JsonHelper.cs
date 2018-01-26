using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.UIBase.Models
{
    public class MyStringEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Action)
            {
                writer.WriteValue(Enum.GetName(typeof(Action), (Action)value));
                return;
            }
            writer.WriteStartObject();

            writer.WritePropertyName("text");
            string displayText = null;
            string desc = null;
            string enumText = null;

            Type type = value.GetType();
            MemberInfo[] members = type.GetMember(value.CastTo<string>());

            var values = Enum.GetValues(type);
            var values2 = (from object v in values select Convert.ToInt32(v)).ToList();
            var intV = Convert.ToInt32(value);

            if (members.Length > 0 && members[0].GetCustomAttributes(typeof(DescriptionAttribute), true).Any())
            {
                desc = members[0].ToDescription();
            }
            enumText = values2.Contains(intV) ? value.CastTo<string>() : "";
            if (members.Length > 0 && members[0].GetCustomAttributes(typeof(DisplayAttribute), true).Any())
            {
                var customAttribute = members[0].GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault();
                if (customAttribute != null)
                    displayText = customAttribute.Name;
            }
            writer.WriteValue(displayText ?? desc ?? enumText);

            if (!string.IsNullOrEmpty(desc) && !string.IsNullOrEmpty(displayText))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(desc);
            }

            writer.WritePropertyName("value");
            writer.WriteValue(Convert.ToInt32(value));

            writer.WriteEndObject();
        }
    }
    public static class JsonHelper
    {
        private static IsoDateTimeConverter GeTimeConverter(string format)
        {
            return new IsoDateTimeConverter() { DateTimeFormat = format };
        }

        public static IsoDateTimeConverter JsonDateConverter
        {
            get
            {
                return new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yyyy" };
            }
        }

        public static IsoDateTimeConverter JsonDateTimeConverter
        {
            get
            {
                return new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yyyy HH:mm:ss" };
            }
        }

        public static StringEnumConverter EnumConverter
        {
            get
            {
                return new MyStringEnumConverter();
            }
        }

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string SerializeObject(object source, bool handleEnum = true)
        {
            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            settings.Converters.Add(JsonDateConverter);
            if (handleEnum)
            {
                settings.Converters.Add(EnumConverter);
            }
#if DEBUG
            settings.Formatting = Formatting.Indented;
#endif
            return JsonConvert.SerializeObject(source, settings);
        }

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="datetimeFormat">The datetime format.</param>
        /// <returns></returns>
        public static string SerializeObject(object source, string datetimeFormat)
        {
            return JsonConvert.SerializeObject(source, GeTimeConverter(datetimeFormat), EnumConverter);
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
      
    }
}