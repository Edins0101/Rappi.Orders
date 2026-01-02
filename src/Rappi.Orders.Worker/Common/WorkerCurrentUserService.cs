using Rappi.Orders.Application.Interfaces;

namespace Rappi.Orders.Worker.Common
{
    public sealed class WorkerCurrentUserService : ICurrentUserService
    {
        public string UserName => "worker-service";
    }
}
