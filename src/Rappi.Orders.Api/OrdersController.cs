using Microsoft.AspNetCore.Mvc;
using Rappi.Orders.Application.Common.Responses;
using Rappi.Orders.Application.Dtos;
using Rappi.Orders.Application.UseCases;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rappi.Orders.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class OrdersController : ControllerBase
    {
        private readonly IOrdersAppService _service;

        public OrdersController(IOrdersAppService service)
        {
            _service = service;
        }

        /// <summary>
        /// Crea un pedido (lista de ítems) y devuelve el total a pagar
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateOrderRequest request,
            CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            return MapResponse(result);
        }

        /// <summary>
        /// Obtiene el total a pagar por AggregatorOrder
        /// </summary>
        [HttpGet("{aggregatorOrder}/total")]
        public async Task<IActionResult> GetTotal(
            [FromRoute] string aggregatorOrder,
            CancellationToken ct)
        {
            var result = await _service.GetTotalAsync(aggregatorOrder, ct);
            return MapResponse(result);
        }

        /// <summary>
        /// Cambia el estado del pedido por AggregatorOrder
        /// </summary>
        [HttpPatch("status")]
        public async Task<IActionResult> ChangeStatus(
            [FromBody] ChangeStatusRequest request,
            CancellationToken ct)
        {
            var result = await _service.ChangeStatusAsync(request, ct);
            return MapResponse(result);
        }

        // 🔁 Mapper centralizado ApiResponse → HTTP
        private IActionResult MapResponse<T>(ApiResponse<T> response)
        {
            return response.Status switch
            {
                ResponseStatus.Success => Ok(response),
                ResponseStatus.ValidationError => BadRequest(response),
                _ => StatusCode(500, response)
            };
        }
    }
}
