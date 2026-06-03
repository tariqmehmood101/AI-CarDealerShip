using CarDealership.Api.Shared.Common;

namespace CarDealership.UnitTests;

public class ResultTests
{
    [Fact]
    public void Success_Result_IsSuccessful()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_Result_HasError()
    {
        var errorMessage = "Something went wrong";
        var result = Result.Failure(errorMessage);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public void Success_GenericResult_HoldsValue()
    {
        var payload = "payload";
        var result = Result<string>.Success(payload);

        Assert.True(result.IsSuccess);
        Assert.Equal(payload, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_GenericResult_HasError()
    {
        var result = Result<string>.Failure("Bad request");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("Bad request", result.Error);
    }
}
