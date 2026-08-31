using System.Collections.ObjectModel;
using System.Data;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace AgroControl.Api.Observability;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, _, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "AgroControl.Api")
                .Enrich.WithProperty("EnvironmentName", context.HostingEnvironment.EnvironmentName)
                .WriteTo.Console();

            var connectionString = context.Configuration.GetConnectionString("AgroControlDatabase");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                loggerConfiguration.WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "ApplicationLogs",
                        AutoCreateSqlTable = false,
                        BatchPostingLimit = 50,
                        BatchPeriod = TimeSpan.FromSeconds(5)
                    },
                    columnOptions: CreateColumnOptions());
            }
        });

        return builder;
    }

    public static WebApplication UseObservability(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (_, _, exception) =>
                exception is null ? LogEventLevel.Information : LogEventLevel.Error;
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
                diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value ?? string.Empty);
                diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
            };
        });

        return app;
    }

    private static ColumnOptions CreateColumnOptions()
    {
        var options = new ColumnOptions();
        options.Id.DataType = SqlDbType.BigInt;
        options.PrimaryKey = null;
        options.TimeStamp.DataType = SqlDbType.DateTimeOffset;
        options.TimeStamp.ConvertToUtc = true;
        options.Store.Add(StandardColumn.LogEvent);
        options.Store.Add(StandardColumn.TraceId);
        options.Store.Add(StandardColumn.SpanId);
        options.AdditionalColumns = new Collection<SqlColumn>
        {
            new() { ColumnName = "CorrelationId", DataType = SqlDbType.NVarChar, DataLength = 64 },
            new() { ColumnName = "RequestMethod", DataType = SqlDbType.NVarChar, DataLength = 16 },
            new() { ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, DataLength = 2048 },
            new() { ColumnName = "StatusCode", DataType = SqlDbType.Int },
            new() { ColumnName = "SourceContext", DataType = SqlDbType.NVarChar, DataLength = 512 },
            new() { ColumnName = "EnvironmentName", DataType = SqlDbType.NVarChar, DataLength = 64 },
            new() { ColumnName = "Application", DataType = SqlDbType.NVarChar, DataLength = 100 }
        };
        return options;
    }
}
