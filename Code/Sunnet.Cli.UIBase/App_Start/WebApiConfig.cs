using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Converters;

namespace Sunnet.Cli.UIBase
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.ParameterBindingRules.Insert(0,SimplePostVariableParameterBinding.HookupParameterBinding);

            config.MessageHandlers.Add(new MethodOverrideHandler());

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
                new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yyyy HH:mm:ss" });
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.XmlFormatter.UseXmlSerializer = true;
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );
        }
    }
}
