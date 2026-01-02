namespace Rappi.Orders.Application.Common.Responses
{
    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; init; }

        private ApiResponse(ResponseStatus status, T? data, IEnumerable<string>? messages)
            : base(status, messages)
        {
            Data = data;
        }

        public static ApiResponse<T> Success(T data, string? message = null)
            => new(ResponseStatus.Success, data, message is null ? null : new[] { message });

        public static ApiResponse<T> Error(params string[] messages)
            => new(ResponseStatus.Error, default, messages);

        public static ApiResponse<T> ValidationError(params string[] messages)
            => new(ResponseStatus.ValidationError, default, messages);
    }
}
