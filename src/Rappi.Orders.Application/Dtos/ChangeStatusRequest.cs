namespace Rappi.Orders.Application.Dtos
{
    public record ChangeStatusRequest(
    string AggregatorOrder,
    string NewStatus
);
}
