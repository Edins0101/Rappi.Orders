namespace Rappi.Orders.Application.Common.Responses
{
    public class ApiResponse
    {
        public ResponseStatus Status { get; init; }
        public List<string> Messages { get; init; } = new();

        protected ApiResponse(ResponseStatus status, IEnumerable<string>? messages = null)
        {
            Status = status;
            if (messages is not null) Messages = messages.ToList();
        }
    }
}
