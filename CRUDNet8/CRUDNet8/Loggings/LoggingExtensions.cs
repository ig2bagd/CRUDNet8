using Serilog.Configuration;
using Serilog;

namespace CRUDNet8.Loggings;

// https://stackoverflow.com/questions/62212569/how-to-get-serilog-to-use-custom-enricher-from-json-config-file
public static class LoggingExtensions
{
    public static LoggerConfiguration WithTimestamp(this LoggerEnrichmentConfiguration enrich)
    {
        if (enrich == null)
            throw new ArgumentNullException(nameof(enrich));

        return enrich.With<DateTimeEnricher>();
    }
}
