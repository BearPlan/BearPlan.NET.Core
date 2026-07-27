

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace BearPlan.Core.Exception
{

    /// <summary>
    /// 业务异常
    /// 注:并不会当作真正的异常处理,仅为方便返回前端错误提示信息
    /// </summary>
    public class BusException :  System.Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="httpStatusCode"></param>
        /// <param name="errors"></param>
        public BusException(
            string message,
            int errorCode = 400,
            int httpStatusCode = StatusCodes.Status200OK,
            Dictionary<string, string> errors = null)
            : base(message)
        {
            ErrorCode = errorCode;
            HttpStatusCode = httpStatusCode;
            Errors = errors;
        }

        public BusException()
        {
            ErrorCode = 400;
            HttpStatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// 业务错误码（给前端用）
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// HTTP 状态码（给协议层用）
        /// </summary>
        public int HttpStatusCode { get; set; }

        /// <summary>
        /// 字段级错误信息
        /// </summary>
        public Dictionary<string, string> Errors { get; set; }




    }
}
