using Serilog.Core;
using Serilog.Events;

namespace CRUDNet8.Enrichers;

// https://github.com/serilog/serilog/issues/1024#issuecomment-338518695
// https://rmauro.dev/serilog-custom-enricher-on-aspnet-core/
// https://medium.com/@kerimemurla/how-to-create-a-serilog-enricher-that-adds-http-information-to-your-logs-1588fc83ccbb
// https://nblumhardt.com/2020/09/serilog-inject-dependencies/
public class TimestampEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
    {
        logEvent.AddPropertyIfAbsent(pf.CreateProperty("UtcTimestamp", logEvent.Timestamp.UtcDateTime));
        logEvent.AddPropertyIfAbsent(pf.CreateProperty("LocalTimestamp", logEvent.Timestamp.LocalDateTime));
    }
}
