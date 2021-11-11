using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class GatewayTimeoutHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.GatewayTimeout;

        public GatewayTimeoutHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.GatewayTimeout, message, innerException)
        {
        }

        protected GatewayTimeoutHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}