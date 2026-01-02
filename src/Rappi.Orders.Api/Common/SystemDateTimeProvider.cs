using Rappi.Orders.Application.Interfaces;

namespace Rappi.Orders.Api.Common
{

    public sealed class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
