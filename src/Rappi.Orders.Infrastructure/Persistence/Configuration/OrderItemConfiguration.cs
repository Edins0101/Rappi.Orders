using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rappi.Orders.Domain.Entities;
using Rappi.Orders.Domain.Enums;
using Rappi.Orders.Domain.ValueObjects;

namespace Rappi.Orders.Infrastructure.Persistence.Configuration
{
    public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("Orders", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                .HasConversion(
                    v => v.Value,
                    v => new OrderCode(v)
                )
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.OrderCode).IsUnique();

            builder.Property(x => x.AggregatorOrder)
                .HasConversion(
                    v => v.Value,
                    v => new AggregatorOrderId(v)
                )
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.AggregatorOrder);

            builder.Property(x => x.Description)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<OrderStatus>(v)
                )
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(x => x.Value)
                .HasConversion(
                    v => v.Amount,
                    v => new Money(v)
                )
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.CancelledAt);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(x => x.UpdatedAt);
        }
    }
}
