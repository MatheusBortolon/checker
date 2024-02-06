using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace checker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices(services =>
                services.AddHealthChecks()
                    .AddCheck<HealthyCheck>(nameof(HealthyCheck))
                    .AddCheck<UnhealthyCheck>(nameof(UnhealthyCheck))
            );

            var app = builder.Build();
            app.Start();

            var config = app.Services.GetService<IConfiguration>();

            Console.WriteLine(config.GetValue<string>("config-test"));

            var healthCheckService = app.Services.GetRequiredService<HealthCheckService>();

            var result = healthCheckService.CheckHealthAsync().Result;
            Console.Write(Helpers.WriteResponse(result));
            
            app.WaitForShutdown();
        }
    }

    internal class HealthyCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
        }
    }

    internal class UnhealthyCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("An unhealthy result."));
        }
    }

    internal class Helpers
    {
        public static string WriteResponse(HealthReport healthReport)
        {
            var options = new JsonWriterOptions { Indented = true };

            using var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString("status", healthReport.Status.ToString());
                jsonWriter.WriteStartObject("results");

                foreach (var healthReportEntry in healthReport.Entries)
                {
                    jsonWriter.WriteStartObject(healthReportEntry.Key);
                    jsonWriter.WriteString("status",
                        healthReportEntry.Value.Status.ToString());
                    jsonWriter.WriteString("description",
                        healthReportEntry.Value.Description);
                    jsonWriter.WriteStartObject("data");

                    foreach (var item in healthReportEntry.Value.Data)
                    {
                        jsonWriter.WritePropertyName(item.Key);

                        JsonSerializer.Serialize(jsonWriter, item.Value,
                            item.Value?.GetType() ?? typeof(object));
                    }

                    jsonWriter.WriteEndObject();
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}
