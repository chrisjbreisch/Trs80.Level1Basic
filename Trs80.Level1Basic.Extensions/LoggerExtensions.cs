using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Trs80.Level1Basic.Extensions;

public static class LoggerExtensions
{
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension Method")]
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    public static string JsonSerializeObject(this ILogger logger, object item)
    {
        // this really should be a regex. I'm too lazy.
        return JsonConvert.SerializeObject(item).Replace(",", ",\r\n").Replace("{\"", "{\r\n\"")
            .Replace("\r\n\"", "\r\n\t\"");
    }
}

