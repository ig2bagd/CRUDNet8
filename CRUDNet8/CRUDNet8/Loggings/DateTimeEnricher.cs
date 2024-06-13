using Serilog.Core;
using Serilog.Events;
using System.Runtime.Serialization;

namespace CRUDNet8.Loggings;

// https://github.com/nblumhardt/serilog-sinks-browserhttp/issues/3
// https://github.com/serilog/serilog/issues/1024#issuecomment-338518695
// https://rmauro.dev/serilog-custom-enricher-on-aspnet-core/
// https://medium.com/@kerimemurla/how-to-create-a-serilog-enricher-that-adds-http-information-to-your-logs-1588fc83ccbb
// https://nblumhardt.com/2020/09/serilog-inject-dependencies/
public class DateTimeEnricher : ILogEventEnricher
{
    private const string _dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff -00:00";
    private const string _dateTimeFormat2 = "MM/dd/yyyy HH:mm:ss.fff";
    private const string _timeFormat = "HH:mm:ss.fff -00:00";
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "UTCDateTimeStamp", DateTime.UtcNow.ToString(_dateTimeFormat)));

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "UTCTimeStamp", DateTime.UtcNow.ToString(_timeFormat)));

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "LocalTimeStamp", logEvent.Timestamp.ToLocalTime().ToString(_dateTimeFormat2)));
    }
}
