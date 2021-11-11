using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class RequestEntityTooLargeHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.RequestEntityTooLarge;

        public RequestEntityTooLargeHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.RequestEntityTooLarge, message, innerException)
        {
        }

        protected RequestEntityTooLargeHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}