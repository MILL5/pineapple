using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class ForbiddenHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.Forbidden;

        public ForbiddenHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.Forbidden, message, innerException)
        {
        }

        protected ForbiddenHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
