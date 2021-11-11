using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class UnauthorizedHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.Unauthorized;

        public UnauthorizedHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.Unautorized, message, innerException)
        {
        }

        protected UnauthorizedHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
