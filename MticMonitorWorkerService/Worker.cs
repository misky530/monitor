using Npgsql;
using Prometheus;

namespace MticMonitorWorkerService;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    private readonly Gauge _recordsGauge = Metrics.CreateGauge("mtic_records", "Number of records in the database");

    // 数据库连接字符串
    private const string ConnectionString =
        "Host=36.137.225.249:5436;Username=postgres;Password=mtic0756-prod;Database=mtic-hd";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            UpdateRecordCountMetric(ConnectionString);
            // 每10秒查询一次数据库, 并更新指标, 性能提升
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private void UpdateRecordCountMetric(string connectionString)
    {
        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("select count(1) from tag_hd where time>'2024-01-01 00:00:00'", conn);
            var count = cmd.ExecuteScalar();
            if (count is not long c)
            {
                logger.LogError("count is not long");
                return;
            }

            _recordsGauge.Set(c);
            logger.LogInformation("Updated record count: {Count}", c);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while trying to query the database.");
        }
    }
}