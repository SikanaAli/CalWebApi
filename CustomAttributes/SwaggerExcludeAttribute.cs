using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SwaggerExcludeAttribute:Attribute
    {
    }
}
