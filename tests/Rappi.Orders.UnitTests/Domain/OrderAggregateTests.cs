using FluentAssertions;
using Rappi.Orders.Domain.Aggregates;
using Rappi.Orders.Domain.Common;
using Rappi.Orders.Domain.Entities;
using Rappi.Orders.Domain.Enums;
using Rappi.Orders.Domain.ValueObjects;

namespace Rappi.Orders.UnitTests.Domain
{
    public class OrderAggregateTests
    {
        private static List<OrderItem> BuildItems(string agg = "ORD-100", OrderStatus status = OrderStatus.Created)
        {
            var aggregator = new AggregatorOrderId(agg);

            return new List<OrderItem>
        {
            new(
                id: Guid.NewGuid(),
                orderCode: new OrderCode("BG-100"),
                aggregatorOrder: aggregator,
                description: "item 1",
                createdAt: DateTimeOffset.UtcNow,
                status: status,
                value: new Money(2.09m),
                createdBy: "tester"
            ),
            new(
                id: Guid.NewGuid(),
                orderCode: new OrderCode("BG-101"),
                aggregatorOrder: aggregator,
                description: "item 2",
                createdAt: DateTimeOffset.UtcNow,
                status: status,
                value: new Money(2.10m),
                createdBy: "tester"
            )
        };
        }

        [Fact]
        public void Total_should_be_rounded_to_2_decimals()
        {
            var agg = new AggregatorOrderId("ORD-200");
            var items = new List<OrderItem>
        {
            new(Guid.NewGuid(), new OrderCode("BG-200"), agg, "a", DateTimeOffset.UtcNow, OrderStatus.Created, new Money(2.095m), "tester"),
            new(Guid.NewGuid(), new OrderCode("BG-201"), agg, "b", DateTimeOffset.UtcNow, OrderStatus.Created, new Money(2.095m), "tester"),
        };

            var aggregate = new OrderAggregate(agg, items);

            aggregate.Total().Amount.Should().Be(4.20m); // 2.10 + 2.10
        }

        [Fact]
        public void Shipped_order_cannot_be_cancelled()
        {
            var items = BuildItems(status: OrderStatus.Shipped);
            var aggregate = new OrderAggregate(new AggregatorOrderId("ORD-100"), items);

            Action act = () => aggregate.ChangeStatus(OrderStatus.Cancelled, "tester", DateTimeOffset.UtcNow);

            act.Should().Throw<DomainException>()
                .WithMessage("*Shipped*no puede ser cancelado*");
        }

        [Fact]
        public void Paid_order_can_only_change_to_Shipped()
        {
            var items = BuildItems(status: OrderStatus.Paid);
            var aggregate = new OrderAggregate(new AggregatorOrderId("ORD-100"), items);

            Action act = () => aggregate.ChangeStatus(OrderStatus.Cancelled, "tester", DateTimeOffset.UtcNow);

            act.Should().Throw<DomainException>()
                .WithMessage("*Paid*solo puede pasar a Shipped*");
        }
    }
}
