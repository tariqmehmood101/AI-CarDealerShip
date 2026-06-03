namespace CarDealership.Api.Shared.Common;

public sealed record ApiErrorResponse(
    string Type,
    string Title,
    int Status,
    string Detail,
    string TraceId
);
