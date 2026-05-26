using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.WebHost.HttpApi.Results
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ApiResult Ok(string message = "Success")
            => new() { Success = true, Message = message };

        public static ApiResult Fail(string message = "Fail")
            => new() { Success = false, Message = message };
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }

        public static ApiResult<T> Ok(T? data, string message = "Success")
            => new() { Success = true, Message = message, Data = data };

        public static new ApiResult<T> Fail(string message = "Fail")
            => new() { Success = false, Message = message };
    }
}