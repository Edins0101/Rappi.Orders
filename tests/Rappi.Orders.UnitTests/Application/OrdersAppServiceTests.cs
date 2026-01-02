using FluentAssertions;
using Rappi.Orders.Application.Common.Responses;
using Rappi.Orders.Application.Dtos;
using Rappi.Orders.Application.UseCases;
using Rappi.Orders.UnitTests.Application.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rappi.Orders.UnitTests.Application
{
    public class OrdersAppServiceTests
    {
        [Fact]
        public async Task CreateAsync_should_return_total_and_success()
        {
            var repo = new FakeOrdersRepository();
            var clock = new FakeClock();
            var user = new FakeUser { UserName = "api-user" };

            var service = new OrdersAppService(repo, clock, user);

            var req = new CreateOrderRequest(
                AggregatorOrder: "ORD-100",
                Items: new()
                {
                new CreateOrderItemDto("BG-100", "item 1", 2.09m),
                new CreateOrderItemDto("BG-101", "item 2", 2.10m)
                }
            );

            var res = await service.CreateAsync(req, CancellationToken.None);

            res.Status.Should().Be(ResponseStatus.Success);
            res.Data!.AggregatorOrder.Should().Be("ORD-100");
            res.Data.Total.Should().Be(4.19m);
        }

        [Fact]
        public async Task ChangeStatusAsync_should_fail_when_Paid_to_Cancelled()
        {
            var repo = new FakeOrdersRepository();
            var clock = new FakeClock();
            var user = new FakeUser { UserName = "api-user" };

            var service = new OrdersAppService(repo, clock, user);

            // primero creamos
            await service.CreateAsync(new CreateOrderRequest(
                "ORD-999",
                new()
                {
                new CreateOrderItemDto("BG-900", "x", 1.00m),
                new CreateOrderItemDto("BG-901", "y", 1.00m)
                }
            ), CancellationToken.None);

            // lo ponemos Paid
            var paidRes = await service.ChangeStatusAsync(new ChangeStatusRequest("ORD-999", "Paid"), CancellationToken.None);
            paidRes.Status.Should().Be(ResponseStatus.Success);

            // intentamos Cancelled (debe fallar por regla)
            var cancelRes = await service.ChangeStatusAsync(new ChangeStatusRequest("ORD-999", "Cancelled"), CancellationToken.None);

            cancelRes.Status.Should().Be(ResponseStatus.ValidationError);
            cancelRes.Messages.Should().Contain(m => m.Contains("Paid", StringComparison.OrdinalIgnoreCase));
        }
    }
}
