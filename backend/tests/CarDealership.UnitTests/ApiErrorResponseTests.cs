using CarDealership.Api.Shared.Common;

namespace CarDealership.UnitTests;

public class ApiErrorResponseTests
{
    [Fact]
    public void ApiErrorResponse_MapsFieldsCorrectly()
    {
        var errorResponse = new ApiErrorResponse(
            Type: "https://httpstatuses.com/500",
            Title: "Internal Server Error",
            Status: 500,
            Detail: "A server side error occurred.",
            TraceId: "abc-123"
        );

        Assert.Equal("https://httpstatuses.com/500", errorResponse.Type);
        Assert.Equal("Internal Server Error", errorResponse.Title);
        Assert.Equal(500, errorResponse.Status);
        Assert.Equal("A server side error occurred.", errorResponse.Detail);
        Assert.Equal("abc-123", errorResponse.TraceId);
    }
}
