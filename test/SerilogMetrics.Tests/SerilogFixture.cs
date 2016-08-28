using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Serilog;
using Serilog.Events;

namespace SerilogMetrics.Tests
{
    public class SerilogFixture : IDisposable
    {

        public SerilogFixture()
        {

            EventsLogged = new List<LogEvent>();

            var configuration = new LoggerConfiguration();
            var logger = configuration
                .MinimumLevel.Verbose()               // Make sure we see also the lowest level
                .Enrich.FromLogContext()
                .WriteTo.Observers(events => events   // So we can check the result
                    .Do(evt => { EventSeen = evt; EventsLogged.Add(evt); })
                    .Subscribe())
                .WriteTo.Console()                    // Still visible in the unit test console
                .CreateLogger();

            Logger = logger;
        }

        public void Dispose()
        {
            EventsLogged.Clear();
            EventSeen = null;

        }

        public ILogger Logger { get; private set; }
        public List<LogEvent> EventsLogged { get; private set; }

        public LogEvent EventSeen { get; private set; }
    }
}