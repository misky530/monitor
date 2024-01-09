using Npgsql;
using Prometheus;

namespace MticMonitorWebApp.Services;

public class MonitorService(ILogger<MonitorService> logger) : IMonitorService
{
    private readonly ILogger<MonitorService> _logger = logger;
    private readonly Gauge _recordsGauge = Metrics.CreateGauge("mtic_records", "Number of records in the database");


    public Task MonitorAsync()
    {
        // 启动 Prometheus 指标服务器
        var metricServer = new MetricServer(hostname: "localhost", port: 1234);
        metricServer.Start();

        // 数据库连接字符串
        var connectionString = "Host=localhost;Username=postgres;Password=your_password;Database=your_db";

        // 检查周期（例如，每10秒检查一次）
        var checkInterval = TimeSpan.FromSeconds(10);

        Console.WriteLine("Monitoring PostgreSQL database...");

        while (true)
        {
            UpdateRecordCountMetric(connectionString);
            Thread.Sleep(checkInterval);
        }

        return Task.CompletedTask;
    }

    private void UpdateRecordCountMetric(string connectionString)
    {
        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM your_table", conn);
            var count = (long)cmd.ExecuteScalar();
            _recordsGauge.Set(count);
            _logger.LogInformation("Updated record count: {Count}", count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to query the database.");
        }
    }
}