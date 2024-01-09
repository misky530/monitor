using MticMonitorWorkerService;
using Prometheus;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
    .Build();

// 启动 Prometheus 指标服务器
var metricServer = new MetricServer(hostname: "localhost", port: 11234);
metricServer.Start();

await host.RunAsync();