using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class TooManyRequestsHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = (HttpStatusCode)429;

        public TooManyRequestsHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.TooManyRequests, message, innerException)
        {
        }

        protected TooManyRequestsHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}