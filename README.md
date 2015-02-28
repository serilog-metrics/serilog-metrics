SerilogMetrics [![Build status](https://ci.appveyor.com/api/projects/status/ou1ofq2vvc0gd0jo/branch/master?svg=true)](https://ci.appveyor.com/project/mivano/serilog-metrics/branch/master)
=================================================================================================================================================

Serilog combines the best features of traditional and structured diagnostic logging in an easy-to-use package and Serilog.Metrics extends this logging framework with measure capabilities like counters, timers, meters and gauges.

* [Serilog Homepage](http://serilog.net)
* [Serilog Documentation](https://github.com/serilog/serilog/wiki)
* [Serilog Metrics Documentation](https://github.com/serilog-metrics/serilog-metrics/wiki)

## Get started
To quickly get started, add the SerilogMetrics package to your solution using the NuGet Package manager or run the following command in the Package Console Window:

```powershell
Install-Package SerilogMetrics
```

The metrics method extensions are extending the ILogger interface of Serilog. So just reference the Serilog namespace and you can invoke the functionality from the logger.

For example;

```csharp
var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();

using (logger.BeginTimedOperation("Time a thread sleep for 2 seconds."))
{
     Thread.Sleep(2000);
}
```

See the [documentation](https://github.com/serilog-metrics/serilog-metrics/wiki) for more details.

Copyright &copy; 2015 Serilog Contributors - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html).
