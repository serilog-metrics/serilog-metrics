using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;

namespace SerilogMetrics.Samples.Console
{
	class Program
	{
		static void Main()
		{
			var logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.ColoredConsole(
					outputTemplate: "{Timestamp:HH:mm:ss} ({ThreadId}) [{Level}] {Message}{NewLine}{Exception}")
				.WriteTo.Trace()
				.Enrich.With(new ThreadIdEnricher(), new MachineNameEnricher())
				.CreateLogger();


			using (logger.BeginTimedOperation("Time a thread sleep for 2 seconds."))
			{
				Thread.Sleep(1000);
				using (logger.BeginTimedOperation("And inside we try a Task.Delay for 2 seconds."))
				{
					Task.Delay(2000).Wait();
				}
				Thread.Sleep(1000);
			}

			using (logger.BeginTimedOperation("Using a passed in identifier", "test-loop"))
			{
				// ReSharper disable once NotAccessedVariable
				var a = "";
				for (var i = 0; i < 1000; i++)
				{
					a += "b";
				}
			}

			// Exceed a limit
			using (logger.BeginTimedOperation("This should execute within 1 second.", null, LogEventLevel.Debug, TimeSpan.FromSeconds(1)))
			{
				Thread.Sleep(1100);
			}

			// Gauge
			var queue = new Queue<int>();
			var gauge = logger.GaugeOperation("queue", "item(s)", () => queue.Count());

			gauge.Write();

			queue.Enqueue(20);

			gauge.Write();

			queue.Dequeue();

			gauge.Write();

			// Counter
			var counter = logger.CountOperation("counter", "operation(s)", true, LogEventLevel.Debug, resolution: 2);
			counter.Increment();
			counter.Increment();
			counter.Increment();
			counter.Decrement();

			System.Console.WriteLine("Press a key to exit.");
			System.Console.ReadKey(true);
		}
	}
}