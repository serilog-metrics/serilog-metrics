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

using System;
using Serilog.Events;
using System.Reactive.Linq;
using Serilog.Context;
using Serilog;
using Xunit;

namespace SerilogMetrics.Tests
{

    public class TimerMeasureTests : IClassFixture<SerilogFixture>
    {

        SerilogFixture fixture;

        public TimerMeasureTests(SerilogFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TimedOperationShouldWriteMessages()
        {
            var check = fixture.Logger.BeginTimedOperation("test", "test-id");

            Assert.Equal("Beginning operation \"test-id\": \"test\"", fixture.EventSeen.RenderMessage());

            check.Dispose();
            Assert.True(fixture.EventSeen.RenderMessage().StartsWith("Completed operation \"test-id\"", StringComparison.Ordinal));

        }

        [Fact]
        public void OperationThatExceedsTimeShouldRenderMessages()
        {
            var check = fixture.Logger.BeginTimedOperation("test", "test-id", LogEventLevel.Information, TimeSpan.FromMilliseconds(2));

            Assert.Equal("Beginning operation \"test-id\": \"test\"", fixture.EventSeen.RenderMessage());

            // Wait at least 30 milliseconds

            System.Threading.Thread.Sleep(30);

            check.Dispose();

            Assert.True(fixture.EventSeen.RenderMessage().Contains("exceeded"));
            Assert.Equal(LogEventLevel.Warning, fixture.EventSeen.Level);
            Assert.True(Convert.ToInt32(fixture.EventSeen.Properties["TimedOperationElapsedInMs"].ToString()) >= 30);
            Assert.True(fixture.EventSeen.Properties.ContainsKey("WarningLimit"));

        }

        [Fact]
        public void CanAddAdditionalProperties()
        {
            var check = fixture.Logger.BeginTimedOperation("test", "test-id");

            using (LogContext.PushProperty("numberOfOperations", 10))
            {

                Assert.Equal("Beginning operation \"test-id\": \"test\"", fixture.EventSeen.RenderMessage());

                check.Dispose();
                Assert.True(fixture.EventSeen.RenderMessage().StartsWith("Completed operation \"test-id\"", StringComparison.Ordinal));
            }

            Assert.True(fixture.EventSeen.Properties.ContainsKey("numberOfOperations"));
            Assert.True(fixture.EventSeen.Properties["numberOfOperations"].ToString() == "10");

        }


    }

}
