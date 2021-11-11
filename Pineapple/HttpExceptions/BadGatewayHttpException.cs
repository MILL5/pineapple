using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class BadGatewayHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.BadGateway;

        public BadGatewayHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.BadGateway, message, innerException)
        {
        }

        protected BadGatewayHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}