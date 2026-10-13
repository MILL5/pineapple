using System;
namespace Pineapple.Threading
{
    [Obsolete("Use System.Diagnostics.Metrics (Counter<T>, Histogram<T>) and dotnet-counters or OpenTelemetry.", DiagnosticId = "PINE004", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public interface ICallsPerMinute
    {
        double Value { get; }
    }
}