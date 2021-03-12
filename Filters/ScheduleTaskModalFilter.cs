using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using CalWebApi.CustomAttributes;
using System.Reflection;

namespace CalWebApi.Filters
{
    public class ScheduleTaskModalFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
                return;

            const BindingFlags bindingFlags = BindingFlags.Public |
                                         BindingFlags.NonPublic |
                                         BindingFlags.Instance;
            //var memberList = context.Type
            //                    .GetFields(bindingFlags).Cast<MemberInfo>()
            //                    .Concat(context.Type
            //                    .GetProperties(bindingFlags));

            //var excludedList = memberList.Where(m =>
            //                                    m.GetCustomAttribute<SwaggerExcludeAttribute>()
            //                                    != null)
            //                             .Select(m =>
            //                                 (m.GetCustomAttribute<SwaggerExcludeAttribute>()
            //                                  ?.TypeId.ToString() ?? m.Name.ToCamelCase()));

            var excludedProperties = context.Type.GetProperties()
                                         .Where(t =>
                                                t.GetCustomAttribute<SwaggerExcludeAttribute>()
                                                != null);

            foreach (var excludedProperty in excludedProperties)
            {
                System.Diagnostics.Debug.WriteLine("PROP => " + excludedProperty.Name);
                if (schema.Properties.ContainsKey(excludedProperty.Name.ToCamelCase()))
                    schema.Properties.Remove(excludedProperty.Name.ToCamelCase());
            }
        }

    }

    internal static class StringExtensions
    {
        internal static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
    }
}
