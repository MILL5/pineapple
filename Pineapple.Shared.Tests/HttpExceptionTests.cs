using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.HttpExceptions;
using System.Net;
using System;

namespace Pineapple.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HttpExceptionTests
    {
        const string CUSTOM_ERR_MESSAGE = "There was an error processing the request";

        const string INTERNAL_ERR_MESSAGE = "Internal error message";
        
        readonly Exception INTERNAL_EXCEPTION = new Exception(INTERNAL_ERR_MESSAGE);

        [TestMethod]
        public void HttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            var httpException1 = new HttpException(statusCode);
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new HttpException(statusCode, HttpErrorMessages.InternalServerError, CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.InternalServerError, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<HttpException>(() => ThrowHttpException(nameof(HttpException)));
        }

        [TestMethod]
        public void BadRequestHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            var httpException1 = new BadRequestHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new BadRequestHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.BadRequest, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<BadRequestHttpException>(() => ThrowHttpException(nameof(BadRequestHttpException)));
        }

        [TestMethod]
        public void NotFoundHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.NotFound;

            var httpException1 = new NotFoundHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new NotFoundHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.NotFound, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<NotFoundHttpException>(() => ThrowHttpException(nameof(NotFoundHttpException)));
        }

        [TestMethod]
        public void ForbiddenHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.Forbidden;

            var httpException1 = new ForbiddenHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new ForbiddenHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.Forbidden, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<ForbiddenHttpException>(() => ThrowHttpException(nameof(ForbiddenHttpException)));
        }

        [TestMethod]
        public void UnauthorizedHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.Unauthorized;

            var httpException1 = new UnauthorizedHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new UnauthorizedHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.Unautorized, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<UnauthorizedHttpException>(() => ThrowHttpException(nameof(UnauthorizedHttpException)));
        }

        [TestMethod]
        public void RequestUriTooLongHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.RequestUriTooLong;

            var httpException1 = new RequestUriTooLongHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new RequestUriTooLongHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.RequestUriTooLong, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<RequestUriTooLongHttpException>(() => ThrowHttpException(nameof(RequestUriTooLongHttpException)));
        }

        [TestMethod]
        public void UnsupportedMediaTypeHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.UnsupportedMediaType;

            var httpException1 = new UnsupportedMediaTypeHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new UnsupportedMediaTypeHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.UnsupportedMediaType, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<UnsupportedMediaTypeHttpException>(() => ThrowHttpException(nameof(UnsupportedMediaTypeHttpException)));
        }

        [TestMethod]
        public void TooManyRequestsHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = (HttpStatusCode)429;

            var httpException1 = new TooManyRequestsHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new TooManyRequestsHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.TooManyRequests, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<TooManyRequestsHttpException>(() => ThrowHttpException(nameof(TooManyRequestsHttpException)));
        }

        [TestMethod]
        public void ServiceUnavailableHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.ServiceUnavailable;

            var httpException1 = new ServiceUnavailableHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new ServiceUnavailableHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.ServiceUnavailable, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<ServiceUnavailableHttpException>(() => ThrowHttpException(nameof(ServiceUnavailableHttpException)));
        }

        [TestMethod]
        public void RequestTimeoutHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.RequestTimeout;

            var httpException1 = new RequestTimeoutHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new RequestTimeoutHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.RequestTimeout, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<RequestTimeoutHttpException>(() => ThrowHttpException(nameof(RequestTimeoutHttpException)));
        }

        [TestMethod]
        public void RequestEntityTooLargeHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.RequestEntityTooLarge;

            var httpException1 = new RequestEntityTooLargeHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new RequestEntityTooLargeHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.RequestEntityTooLarge, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<RequestEntityTooLargeHttpException>(() => ThrowHttpException(nameof(RequestEntityTooLargeHttpException)));
        }

        [TestMethod]
        public void NotImplementedHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.NotImplemented;

            var httpException1 = new NotImplementedHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new NotImplementedHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.NotImplemented, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<NotImplementedHttpException>(() => ThrowHttpException(nameof(NotImplementedHttpException)));
        }

        [TestMethod]
        public void MethodNotAllowedHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.MethodNotAllowed;

            var httpException1 = new MethodNotAllowedHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new MethodNotAllowedHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.MethodNotAllowed, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<NotImplementedHttpException>(() => ThrowHttpException(nameof(NotImplementedHttpException)));
        }

        [TestMethod]
        public void GatewayTimeoutHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.GatewayTimeout;

            var httpException1 = new GatewayTimeoutHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new GatewayTimeoutHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.GatewayTimeout, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<GatewayTimeoutHttpException>(() => ThrowHttpException(nameof(GatewayTimeoutHttpException)));
        }

        [TestMethod]
        public void ConflictHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.Conflict;

            var httpException1 = new ConflictHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new ConflictHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.Conflict, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<ConflictHttpException>(() => ThrowHttpException(nameof(ConflictHttpException)));
        }

        [TestMethod]
        public void BadGatewayHttpExceptionThrows()
        {
            const HttpStatusCode statusCode = HttpStatusCode.BadGateway;

            var httpException1 = new BadGatewayHttpException();
            Assert.AreEqual(statusCode, httpException1.StatusCode);
            Assert.AreEqual(HttpErrorMessages.DefaultCustomMessage, httpException1.Message);
            Assert.AreEqual(null, httpException1.InnerException);

            var httpException2 = new BadGatewayHttpException(CUSTOM_ERR_MESSAGE, INTERNAL_EXCEPTION);

            Assert.AreEqual(statusCode, httpException2.StatusCode);
            Assert.AreEqual(HttpErrorMessages.BadGateway, httpException2.ErrorMessage);
            Assert.AreEqual(CUSTOM_ERR_MESSAGE, httpException2.Message);
            Assert.AreEqual(INTERNAL_EXCEPTION, httpException2.InnerException);
            Assert.AreEqual(INTERNAL_ERR_MESSAGE, httpException2.InnerException.Message);

            Assert.ThrowsException<BadGatewayHttpException>(() => ThrowHttpException(nameof(BadGatewayHttpException)));
        }

        private void ThrowHttpException(string exceptionName)
        {
            switch (exceptionName)
            {
                case nameof(HttpException):
                    throw new HttpException(HttpStatusCode.InternalServerError);

                case nameof(BadRequestHttpException):
                    throw new BadRequestHttpException();

                case nameof(NotFoundHttpException):
                    throw new NotFoundHttpException();

                case nameof(ForbiddenHttpException):
                    throw new ForbiddenHttpException();

                case nameof(UnauthorizedHttpException):
                    throw new UnauthorizedHttpException();

                case nameof(RequestUriTooLongHttpException):
                    throw new RequestUriTooLongHttpException();

                case nameof(UnsupportedMediaTypeHttpException):
                    throw new UnsupportedMediaTypeHttpException();

                case nameof(TooManyRequestsHttpException):
                    throw new TooManyRequestsHttpException();

                case nameof(ServiceUnavailableHttpException):
                    throw new ServiceUnavailableHttpException();

                case nameof(RequestTimeoutHttpException):
                    throw new RequestTimeoutHttpException();

                case nameof(RequestEntityTooLargeHttpException):
                    throw new RequestEntityTooLargeHttpException();

                case nameof(NotImplementedHttpException):
                    throw new NotImplementedHttpException();

                case nameof(MethodNotAllowedHttpException):
                    throw new MethodNotAllowedHttpException();

                case nameof(GatewayTimeoutHttpException):
                    throw new GatewayTimeoutHttpException();

                case nameof(ConflictHttpException):
                    throw new ConflictHttpException();

                case nameof(BadGatewayHttpException):
                    throw new BadGatewayHttpException();

                default:
                    break;
            }
        }
    }
}