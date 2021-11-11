using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class BadRequestHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.BadRequest;

        public BadRequestHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.BadRequest, message, innerException)
        {
        }

        protected BadRequestHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
