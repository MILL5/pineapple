using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class ServiceUnavailableHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.ServiceUnavailable;

        public ServiceUnavailableHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.ServiceUnavailable, message, innerException)
        {
        }

        protected ServiceUnavailableHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}