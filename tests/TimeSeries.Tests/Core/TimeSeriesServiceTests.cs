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

    [Fact]
    public async Task Delete_WhenRecordExists_DeletesViaRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);
        var sut = GetSut();

        // Act
        await sut.DeleteAsync(id);

        // Assert
        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task Delete_WhenRecordDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(x => x.DeleteAsync(id)).ReturnsAsync(false);
        var sut = GetSut();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.DeleteAsync(id));

        // Assert
        Assert.Equal($"Time series record '{id}' was not found.", exception.Message);
    }

    [Fact]
    public async Task Update_WhenRecordIsInvalid_ThrowsAndDoesNotPersist()
    {
        // Arrange
        var id = Guid.NewGuid();
        // production requires an energy source
        var invalidRecord = ValidRecord() with { EnergySource = null };
        var sut = GetSut();

        // Act
        await Assert.ThrowsAsync<RecordValidationException>(() => sut.UpdateAsync(id, invalidRecord));

        // Assert
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<TimeSeriesRecord>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenRecordDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var record = ValidRecord();
        TimeSeriesRecord? nullRecord = null;
        _mockRepository.Setup(x => x.UpdateAsync(id, record)).ReturnsAsync(nullRecord);
        var sut = GetSut();

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.UpdateAsync(id, record));

        // Assert
        Assert.Equal($"Time series record '{id}' was not found.", exception.Message);
    }

    [Fact]
    public async Task Update_WhenRecordExists_ReturnsUpdatedRecord()
    {
        // Arrange
        var id = Guid.NewGuid();
        var record = ValidRecord();
        var stored = record with { Id = id };
        _mockRepository.Setup(x => x.UpdateAsync(id, record)).ReturnsAsync(stored);
        var sut = GetSut();

        // Act
        var result = await sut.UpdateAsync(id, record);

        // Assert
        Assert.Equal(stored, result);
        _mockRepository.Verify(x => x.UpdateAsync(id, record), Times.Once);
    }

    private static TimeSeriesRecord ValidRecord() => new()
    {
        Timestamp = new DateTimeOffset(2026, 11, 21, 12, 0, 0, TimeSpan.Zero),
        Type = RecordType.Production,
        BiddingZone = BiddingZone.DK1,
        EnergySource = EnergySource.Wind,
        Value = 100m
    };


    private TimeSeriesService GetSut()
    {
        var sut = new TimeSeriesService(_mockRepository.Object);
        return sut;
    }
}
