using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class MethodNotAllowedHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.MethodNotAllowed;

        public MethodNotAllowedHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.MethodNotAllowed, message, innerException)
        {
        }

        protected MethodNotAllowedHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}