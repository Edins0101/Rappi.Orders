using Rappi.Orders.Application.Interfaces;

namespace Rappi.Orders.Api.Common
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        public string UserName => "api-user";
    }
}
