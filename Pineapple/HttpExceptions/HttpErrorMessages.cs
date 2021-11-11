namespace Pineapple.HttpExceptions
{
    public static class HttpErrorMessages
    {
        public const string DefaultCustomMessage = "There was an error processing the request";

        public const string InternalServerError = "Internal Server Error";

        public const string BadRequest = "Bad Request";

        public const string NotFound = "Not Found";

        public const string Forbidden = "Forbidden";

        public const string Unautorized = "Unauthorized";

        public const string Conflict = "Conflict";

        public const string RequestTimeout = "Request Timeout";

        public const string MethodNotAllowed = "Method Not Allowed";

        public const string RequestEntityTooLarge = "Payload Too Large";

        public const string RequestUriTooLong = "URI Too Long";

        public const string TooManyRequests = "Too Many Requests";

        public const string NotImplemented = "Not Implemented";

        public const string BadGateway = "Bad Gateway";

        public const string ServiceUnavailable = "Service Unavailable";

        public const string GatewayTimeout = "Gateway Timeout";

        public const string UnsupportedMediaType = "Unsupported Media Type";
    }
}