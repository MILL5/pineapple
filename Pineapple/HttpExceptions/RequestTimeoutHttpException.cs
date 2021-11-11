using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class RequestTimeoutHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.RequestTimeout;

        public RequestTimeoutHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.RequestTimeout, message, innerException)
        {
        }

        protected RequestTimeoutHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}