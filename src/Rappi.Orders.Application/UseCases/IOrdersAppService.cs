using Rappi.Orders.Application.Common.Responses;
using Rappi.Orders.Application.Dtos;

namespace Rappi.Orders.Application.UseCases
{
    public interface IOrdersAppService
    {
        Task<ApiResponse<OrderTotalResponse>> CreateAsync(CreateOrderRequest request, CancellationToken ct);
        Task<ApiResponse<OrderTotalResponse>> GetTotalAsync(string aggregatorOrder, CancellationToken ct);
        Task<ApiResponse<object>> ChangeStatusAsync(ChangeStatusRequest request, CancellationToken ct);
    }
}
