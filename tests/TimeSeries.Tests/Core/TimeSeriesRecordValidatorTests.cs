using TimeSeries.Core.Exceptions;
using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Tests.Core;

public class TimeSeriesRecordValidatorTests
{
    private readonly TimeSeriesRecordValidator sut = new();

    [Theory]
    // flow without target zone
    [InlineData(RecordType.Flow, null, null)]
    // flow to same zone
    [InlineData(RecordType.Flow, BiddingZone.DK1, null)]
    // flow with energy source
    [InlineData(RecordType.Flow, BiddingZone.DK2, EnergySource.Wind)]
    // production without source
    [InlineData(RecordType.Production, null, null)]
    // production with target zone
    [InlineData(RecordType.Production, BiddingZone.DK2, EnergySource.Wind)]
    // consumption with source
    [InlineData(RecordType.Consumption, null, EnergySource.Wind)]
    // consumption with target zone
    [InlineData(RecordType.Consumption, BiddingZone.DK2, null)]
    public void Validate_InvalidTypeCombination_Throws(RecordType type, BiddingZone? toZone, EnergySource? source)
    {
        // Arrange
        var record = TestData.ValidRecord() with { Type = type, ToBiddingZone = toZone, EnergySource = source };

        // Act & Assert
        Assert.Throws<RecordValidationException>(() => sut.Validate(record));
    }

    [Fact]
    public void Validate_SeveralViolations_ReportsAllOfThemPerField()
    {
        // Arrange: negative value, not on the hour, and a production without a source
        var record = TestData.ValidRecord() with
        {
            Value = -1,
            Timestamp = new DateTimeOffset(2026, 11, 21, 12, 30, 0, TimeSpan.Zero),
            EnergySource = null
        };

        // Act
        var exception = Assert.Throws<RecordValidationException>(() => sut.Validate(record));

        // Assert
        Assert.Equal(["EnergySource", "Timestamp", "Value"], exception.Errors.Keys.Order());
    }

    [Theory]
    [InlineData(RecordType.Production, null, EnergySource.Wind)]
    [InlineData(RecordType.Consumption, null, null)]
    [InlineData(RecordType.Flow, BiddingZone.DK2, null)]
    public void Validate_ValidTypeCombination_DoesNotThrow(RecordType type, BiddingZone? toZone, EnergySource? source)
    {
        var record = TestData.ValidRecord() with { Type = type, ToBiddingZone = toZone, EnergySource = source };

        var exception = Record.Exception(() => sut.Validate(record));

        Assert.Null(exception);
    }
}