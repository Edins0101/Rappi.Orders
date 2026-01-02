using Rappi.Orders.Application.Interfaces;

namespace Rappi.Orders.UnitTests.Application.Fakes
{
    public sealed class FakeClock : IDateTimeProvider
    {
        public DateTimeOffset UtcNow { get; set; } = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    }

    public sealed class FakeUser : ICurrentUserService
    {
        public string UserName { get; set; } = "tester";
    }
}
