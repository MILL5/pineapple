using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class HttpException : Exception
    {
        public HttpException(
            HttpStatusCode statusCode,
            string errorMessage = null,
            string message = HttpErrorMessages.DefaultCustomMessage, 
            Exception innerException = null)
                : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        protected HttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public HttpStatusCode StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}