using Rappi.Orders.Application.Interfaces;

namespace Rappi.Orders.Worker.Common
{
    public sealed class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
