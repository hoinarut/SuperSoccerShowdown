using Microsoft.Extensions.Logging;
using Moq;

namespace MyApp.Testing.Common;

public abstract class BaseUnitTests
{
    protected static T GetStrictMock<T>()
        where T : class
        => Mock.Of<T>(MockBehavior.Strict);

    protected static void SetupLoggerMock<T>(Mock<ILogger<T>> loggerMock, LogLevel logLevel)
    {
        loggerMock.Setup(x => x.Log(logLevel,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!))
            .Verifiable();
    }
    
    protected static void VerifyLog<T>(
        Mock<ILogger<T>> loggerMock,
        LogLevel logLevel,
        Times times,
        string? messageContains = null)
    {
        loggerMock.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    messageContains == null || state.ToString()!.Contains(messageContains)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
    
    protected abstract void VerifyAllMocks();
}