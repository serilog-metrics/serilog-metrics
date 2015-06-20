// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using NUnit.Framework;
using System;
using Serilog.Events;
using System.Reactive.Linq;
using Serilog;

namespace SerilogMetrics.Tests
{
    using System.Collections.Generic;

    [TestFixture ()]
	public class CounterMeasureTests
	{
		LogEvent _eventSeen;

        private List<LogEvent> _eventsLogged = new List<LogEvent>();

		public CounterMeasureTests ()
		{
			var configuration = new LoggerConfiguration();
			var logger = configuration
				.MinimumLevel.Verbose()               // Make sure we see also the lowest level
				.WriteTo.Observers(events => events   // So we can check the result
					.Do(evt => { _eventSeen = evt; _eventsLogged.Add(evt); })
					.Subscribe())
				.WriteTo.Console()                    // Still visible in the unit test console
				.CreateLogger();

			Log.Logger = logger;
		}

		[Test ()]
		public void CounterStoresValue ()
		{
			var check = Log.Logger.CountOperation("invocations", "times", false);

			Assert.AreEqual (check.Value (), 0);

			check.Increment ();
			Assert.AreEqual (check.Value (), 1);

			check.Increment ();
			Assert.AreEqual (check.Value (), 2);

			check.Decrement ();
			Assert.AreEqual (check.Value (), 1);

			check.Reset ();
			Assert.AreEqual (check.Value (), 0);

            _eventsLogged.Clear();

			}

		[Test ()]
		public void CounterWritesResult ()
		{
			var check = Log.Logger.CountOperation("invocations", "times", false);

			Assert.AreEqual (check.Value (), 0);

			check.Increment ();
			Assert.AreEqual (check.Value (), 1);
		
			check.Write ();

			Assert.AreEqual (LogEventLevel.Information, _eventSeen.Level);
            Assert.AreEqual ("\"invocations\" count = 1 times at 1 resolution", _eventSeen.RenderMessage());

            _eventsLogged.Clear();
		}

		[Test ()]
		public void CounterWithCustomLevelWritesWithThatLevel ()
		{
			var check = Log.Logger.CountOperation("invocations", "times", false, LogEventLevel.Debug);
		

			check.Write ();

			Assert.AreEqual (LogEventLevel.Debug, _eventSeen.Level);

            _eventsLogged.Clear();
		}

		[Test ()]
		public void CounterWritesDirectResultsToLogger ()
		{
			var check = Log.Logger.CountOperation("invocations", "times", true);

			check.Increment ();
            Assert.AreEqual ("\"invocations\" count = 1 times at 1 resolution", _eventSeen.RenderMessage());

			check.Increment ();
            Assert.AreEqual ("\"invocations\" count = 2 times at 1 resolution", _eventSeen.RenderMessage());

			check.Decrement ();
            Assert.AreEqual ("\"invocations\" count = 1 times at 1 resolution", _eventSeen.RenderMessage());

			check.Reset ();
            Assert.AreEqual ("\"invocations\" count = 0 times at 1 resolution", _eventSeen.RenderMessage());

            _eventsLogged.Clear();
		}

        [Test()]
        public void CounterWritesAtSpecifiedResolution()
        {
            var check = Log.Logger.CountOperation("invocations", "times", true, resolution: 3);
            
            for (var i = 0; i < 20; i++)
            {
                check.Increment();
            }

            Assert.AreEqual(_eventsLogged.Count, 6);

            Assert.AreEqual("\"invocations\" count = 18 times at 3 resolution", _eventSeen.RenderMessage());

            _eventsLogged.Clear();
        }
	}
	
}
