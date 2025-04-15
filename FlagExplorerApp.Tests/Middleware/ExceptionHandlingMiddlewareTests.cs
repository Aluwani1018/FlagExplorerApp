using FlagExplorerApp.Application.Common.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FlagExplorerApp.Tests.Middleware
{
    [TestFixture]
    public class ExceptionHandlingMiddlewareTests
    {
        private Mock<RequestDelegate> _nextMock;
        private Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
        private ExceptionHandlingMiddleware _middleware;

        [SetUp]
        public void SetUp()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Middleware_ShouldReturnBadRequest_WhenOperationCanceledExceptionIsThrown()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new OperationCanceledException());
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task Middleware_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new ArgumentException("Invalid argument"));
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task Middleware_ShouldReturnInternalServerError_WhenUnhandledExceptionIsThrown()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new Exception("Unexpected error"));
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(500);
        }

        [Test]
        public async Task Middleware_ShouldLogWarning_WhenOperationCanceledExceptionIsThrown()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new OperationCanceledException());
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("The operation was canceled by the client.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task Middleware_ShouldLogError_WhenUnhandledExceptionIsThrown()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new Exception("Unexpected error"));
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("An unexpected error occurred")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task Middleware_ShouldPassThrough_WhenNoExceptionIsThrown()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(200); // Assuming successful requests default to 200
            var responseBody = await GetResponseBodyAsync(context);
            responseBody.Should().BeEmpty();
        }

        private async Task<string> GetResponseBodyAsync(HttpContext context)
        {
            context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
            return await new System.IO.StreamReader(context.Response.Body).ReadToEndAsync();
        }
    }
}
