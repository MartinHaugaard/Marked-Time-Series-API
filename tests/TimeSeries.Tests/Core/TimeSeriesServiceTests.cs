using Moq;
using TimeSeries.Core.Exceptions;
using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Tests.Core;

public class TimeSeriesServiceTests
{
    private readonly Mock<ITimeSeriesRepository> _mockRepository;

    public TimeSeriesServiceTests()
    {
        _mockRepository = new Mock<ITimeSeriesRepository>();
    }

    [Fact]
    public void Get_WhenRecordDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        TimeSeriesRecord? nullRecord = null;
        _mockRepository.Setup(x => x.Get(id)).Returns(nullRecord);
        var sut = GetSut();

        // Act
        var response = Assert.Throws<NotFoundException>(() => sut.Get(id));

        // Assert
        Assert.Equal($"Time series record '{id}' was not found.", response.Message);
    }

    private TimeSeriesService GetSut()
    {
        var sut = new TimeSeriesService(_mockRepository.Object);
        return sut;
    }
}
