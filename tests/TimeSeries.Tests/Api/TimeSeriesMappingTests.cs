using TimeSeries.Api.Mapping;
using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Tests.Api
{
    public class TimeSeriesMappingTests
    {
        [Fact]
        public void ToResponse_GivenTimeSeriesRecord_GetsTimeSeriesResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            TimeSeriesRecord model = new TimeSeriesRecord
            {
                Id = id,
                Timestamp = new DateTime(2026, 12, 24, 0, 0, 0),
                Type = RecordType.Consumption,
                BiddingZone = BiddingZone.DK1,
                Value = 90,
            };

            // Act
            var response = model.ToResponse();

            // Assert
            Assert.Equal(id, response.Id);
        }

        [Fact]
        public void ToResponse_RecordWithoutId_ThrowsArgumentException()
        {
            // Arrange
            Guid? id = null;
            TimeSeriesRecord model = new TimeSeriesRecord
            {
                Id = id,
                Timestamp = new DateTime(2026, 12, 24, 0, 0, 0),
                Type = RecordType.Consumption,
                BiddingZone = BiddingZone.DK1,
                Value = 90,
            };


            // Act
            var response = Assert.Throws<InvalidOperationException>(() => model.ToResponse());

            // Assert
            Assert.Equal("Record has no id", response.Message);
        }
    }
}
