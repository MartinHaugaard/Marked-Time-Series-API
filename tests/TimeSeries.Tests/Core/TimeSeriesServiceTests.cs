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
    public async Task Get_WhenRecordDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        TimeSeriesRecord? nullRecord = null;
        _mockRepository.Setup(x => x.GetAsync(id)).ReturnsAsync(nullRecord);
        var sut = GetSut();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetAsync(id));

        // Assert
        Assert.Equal($"Time series record '{id}' was not found.", exception.Message);
    }

    [Fact]
    public async Task Create_WhenRecordIsInvalid_ThrowsAndDoesNotPersist()
    {
        // Arrange
        var invalidRecord = new TimeSeriesRecord
        {
            Timestamp = new DateTimeOffset(2026, 11, 21, 12, 0, 0, TimeSpan.Zero),
            Type = RecordType.Flow,
            BiddingZone = BiddingZone.DK1,
            ToBiddingZone = null, // a flow requires a target zone
            Value = 100m
        };
        var sut = GetSut();

        // Act
        await Assert.ThrowsAsync<RecordValidationException>(() => sut.CreateAsync(invalidRecord));

        // Assert
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<TimeSeriesRecord>()), Times.Never);
    }

    private TimeSeriesService GetSut()
    {
        var sut = new TimeSeriesService(_mockRepository.Object);
        return sut;
    }
}
