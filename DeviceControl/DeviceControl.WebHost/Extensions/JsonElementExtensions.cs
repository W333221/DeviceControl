using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeviceControl.WebHost.Extensions
{
    public static class JsonElementExtensions
    {
        public static T DeserializeSafe<T>(this JsonElement element, JsonSerializerOptions options = null)
        {
            try
            {
                if (element.ValueKind == JsonValueKind.Null ||
                    element.ValueKind == JsonValueKind.Undefined)
                {
                    return default;
                }

                options ??= new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }  // 添加枚举转换器
                };

                return element.Deserialize<T>(options);
            }
            catch (Exception ex)
            {
                Log.Error($"反序列化异常: {ex.Message}");
                Log.Error($"原始数据: {element.GetRawText()}");
                return default;
            }
        }
    }
}