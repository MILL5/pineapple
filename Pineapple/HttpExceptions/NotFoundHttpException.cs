using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class NotFoundHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.NotFound;

        public NotFoundHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.NotFound, message, innerException)
        {
        }

        protected NotFoundHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
