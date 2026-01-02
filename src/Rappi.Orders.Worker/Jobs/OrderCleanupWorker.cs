using Microsoft.Extensions.Options;
using Rappi.Orders.Application.Interfaces;
using Rappi.Orders.Domain.Aggregates;
using Rappi.Orders.Domain.Enums;
using Rappi.Orders.Domain.ValueObjects;

namespace Rappi.Orders.Worker.Jobs
{

    public sealed class OrderCleanupWorker : BackgroundService
    {
        private readonly ILogger<OrderCleanupWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly WorkerOptions _options;

        public OrderCleanupWorker(
            ILogger<OrderCleanupWorker> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<WorkerOptions> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderCleanupWorker started. Every {Hours}h, cancel after {Days} days.",
                _options.RunEveryHours, _options.CancelAfterDays);

            // Corre al iniciar, y luego cada N horas
            await RunOnce(stoppingToken);

            var interval = _options.RunEverySeconds > 0
                ? TimeSpan.FromSeconds(_options.RunEverySeconds)
                : TimeSpan.FromHours(_options.RunEveryHours);

            using var timer = new PeriodicTimer(interval);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunOnce(stoppingToken);
            }
        }

        private async Task RunOnce(CancellationToken ct)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var repo = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
                var clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
                var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

                var cutoff = clock.UtcNow.AddDays(-_options.CancelAfterDays);

                var oldCreatedItems = await repo.GetCreatedOlderThanAsync(cutoff, ct);

                if (oldCreatedItems.Count == 0)
                {
                    _logger.LogInformation("No Created orders older than {Cutoff}.", cutoff);
                    return;
                }

                // Agrupar por AggregatorOrder y aplicar regla como agregado
                var groups = oldCreatedItems
                    .GroupBy(i => i.AggregatorOrder.Value)
                    .ToList();

                var totalGroups = groups.Count;
                var totalItems = oldCreatedItems.Count;

                foreach (var g in groups)
                {
                    var aggId = new AggregatorOrderId(g.Key);

                    // IMPORTANTE: buscamos TODOS los items del pedido, no solo los viejos
                    // para mantener consistencia de estado a nivel agregado.
                    var allItemsForAgg = await repo.GetByAggregatorAsync(aggId, ct);
                    if (allItemsForAgg.Count == 0) continue;

                    var aggregate = new OrderAggregate(aggId, allItemsForAgg);

                    // Solo cancela si el estado actual del agregado es Created
                    if (aggregate.CurrentStatus != OrderStatus.Created)
                        continue;

                    aggregate.ChangeStatus(
                        OrderStatus.Cancelled,
                        currentUser.UserName,
                        clock.UtcNow
                    );
                }

                var changed = await repo.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "Cleanup done. Groups: {Groups}, Items scanned: {Items}, EF changes: {Changes}.",
                    totalGroups, totalItems, changed);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("OrderCleanupWorker cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCleanupWorker failed.");
            }
        }
    }

    public sealed class WorkerOptions
    {
        public int RunEveryHours { get; set; } = 8;
        public int RunEverySeconds { get; set; } = 0;
        public int CancelAfterDays { get; set; } = 14;
    }
}
