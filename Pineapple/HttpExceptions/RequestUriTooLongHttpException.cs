using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class RequestUriTooLongHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.RequestUriTooLong;

        public RequestUriTooLongHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.RequestUriTooLong, message, innerException)
        {
        }

        protected RequestUriTooLongHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}