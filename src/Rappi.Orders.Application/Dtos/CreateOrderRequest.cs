namespace Rappi.Orders.Application.Dtos
{
    public record CreateOrderItemDto(
    string OrderCode,
    string Description,
    decimal Value
);

    public record CreateOrderRequest(
        string AggregatorOrder,
        List<CreateOrderItemDto> Items
    );

    public record OrderTotalResponse(
        string AggregatorOrder,
        decimal Total
    );
}
