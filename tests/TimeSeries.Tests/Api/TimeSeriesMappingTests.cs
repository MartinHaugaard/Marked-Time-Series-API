using TimeSeries.Api.Dtos;
using TimeSeries.Api.Mapping;
using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Tests.Api
{
    public class TimeSeriesMappingTests
    {
        [Fact]
        public void ToResponse_MapsAllFields()
        {
            // Arrange
            // Both optional fields are set to check they're mapped; mapping doesn't validate combinations.
            var record = new TimeSeriesRecord
            {
                Id = Guid.NewGuid(),
                Timestamp = new DateTimeOffset(2026, 12, 24, 12, 0, 0, TimeSpan.Zero),
                Type = RecordType.Flow,
                BiddingZone = BiddingZone.DK1,
                ToBiddingZone = BiddingZone.DK2,
                EnergySource = EnergySource.Wind,
                Value = 90.5m
            };

            // Act
            var response = record.ToResponse();

            // Assert
            Assert.Equal(record.Id, response.Id);
            Assert.Equal(record.Timestamp, response.Timestamp);
            Assert.Equal(record.Type, response.Type);
            Assert.Equal(record.BiddingZone, response.BiddingZone);
            Assert.Equal(record.ToBiddingZone, response.ToBiddingZone);
            Assert.Equal(record.EnergySource, response.EnergySource);
            Assert.Equal(record.Value, response.Value);
        }

        [Fact]
        public void ToModel_FromCreateRequest_MapsAllFieldsAndLeavesIdEmpty()
        {
            // Arrange
            var request = new CreateTimeSeriesRequest
            {
                Timestamp = new DateTimeOffset(2026, 12, 24, 12, 0, 0, TimeSpan.Zero),
                Type = RecordType.Flow,
                BiddingZone = BiddingZone.DK1,
                ToBiddingZone = BiddingZone.DK2,
                EnergySource = EnergySource.Wind,
                Value = 90.5m
            };

            // Act
            var model = request.ToModel();

            // Assert
            Assert.Null(model.Id);
            Assert.Equal(request.Timestamp, model.Timestamp);
            Assert.Equal(request.Type, model.Type);
            Assert.Equal(request.BiddingZone, model.BiddingZone);
            Assert.Equal(request.ToBiddingZone, model.ToBiddingZone);
            Assert.Equal(request.EnergySource, model.EnergySource);
            Assert.Equal(request.Value, model.Value);
        }

        [Fact]
        public void ToResponseList_MapsEveryRecord()
        {
            // Arrange
            var records = new[]
            {
                TestData.ValidRecord() with { Id = Guid.NewGuid() },
                TestData.ValidRecord() with { Id = Guid.NewGuid() }
            };

            // Act
            var responses = records.ToResponseList();

            // Assert
            Assert.Equal(records.Select(r => r.Id), responses.Select(r => (Guid?)r.Id));
        }

        [Fact]
        public void ToResponse_RecordWithoutId_ThrowsInvalidOperationException()
        {
            // Arrange
            var record = TestData.ValidRecord() with { Id = null };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => record.ToResponse());
            Assert.Equal("Record has no id", exception.Message);
        }
    }
}
