using System;
using System.Net;
using System.Runtime.Serialization;

namespace Pineapple.HttpExceptions
{
    [Serializable]
    public class ConflictHttpException : HttpException
    {
        public const HttpStatusCode HTTP_STATUS_CODE = HttpStatusCode.Conflict;

        public ConflictHttpException(string message = HttpErrorMessages.DefaultCustomMessage, Exception innerException = null)
            : base(HTTP_STATUS_CODE, HttpErrorMessages.Conflict, message, innerException)
        {
        }

        protected ConflictHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}