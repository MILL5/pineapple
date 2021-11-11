using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class NotImplementedHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.NotImplemented;

        public NotImplementedHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.NotImplemented, message, innerException)
        {
        }

        protected NotImplementedHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}