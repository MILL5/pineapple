using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class UnsupportedMediaTypeHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.UnsupportedMediaType;

        public UnsupportedMediaTypeHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.UnsupportedMediaType, message, innerException)
        {
        }

        protected UnsupportedMediaTypeHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}