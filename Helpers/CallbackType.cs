﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Helpers
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CallbackType
    {
        URL
    }
}
