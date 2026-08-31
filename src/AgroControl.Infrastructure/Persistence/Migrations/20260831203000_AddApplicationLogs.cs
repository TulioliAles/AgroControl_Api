using AgroControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroControl.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AgroControlDbContext))]
[Migration("20260831203000_AddApplicationLogs")]
public partial class AddApplicationLogs : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE [dbo].[ApplicationLogs] (
                [Id] bigint IDENTITY(1,1) NOT NULL,
                [Message] nvarchar(max) NULL,
                [MessageTemplate] nvarchar(max) NULL,
                [Level] nvarchar(16) NULL,
                [TimeStamp] datetimeoffset NOT NULL,
                [Exception] nvarchar(max) NULL,
                [Properties] xml NULL,
                [LogEvent] nvarchar(max) NULL,
                [TraceId] varchar(32) NULL,
                [SpanId] varchar(16) NULL,
                [CorrelationId] nvarchar(64) NULL,
                [RequestMethod] nvarchar(16) NULL,
                [RequestPath] nvarchar(2048) NULL,
                [StatusCode] int NULL,
                [SourceContext] nvarchar(512) NULL,
                [EnvironmentName] nvarchar(64) NULL,
                [Application] nvarchar(100) NULL
            );
            CREATE INDEX [IX_ApplicationLogs_TimeStamp] ON [dbo].[ApplicationLogs] ([TimeStamp] DESC);
            CREATE INDEX [IX_ApplicationLogs_CorrelationId] ON [dbo].[ApplicationLogs] ([CorrelationId]);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TABLE [dbo].[ApplicationLogs];");
    }
}
