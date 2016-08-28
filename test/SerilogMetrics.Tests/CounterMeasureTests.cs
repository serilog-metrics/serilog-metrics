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

using Serilog.Events;
using Serilog;
using Xunit;

namespace SerilogMetrics.Tests
{
    public class CounterMeasureTests : IClassFixture<SerilogFixture>
    {

        SerilogFixture fixture;

        public CounterMeasureTests(SerilogFixture fixture)
        {
            this.fixture = fixture;
        }


        [Fact]
        public void CounterStoresValue()
        {
            var check = fixture.Logger.CountOperation("invocations", "times", false);

            Assert.Equal(check.Value(), 0);

            check.Increment();
            Assert.Equal(check.Value(), 1);

            check.Increment();
            Assert.Equal(check.Value(), 2);

            check.Decrement();
            Assert.Equal(check.Value(), 1);

            check.Reset();
            Assert.Equal(check.Value(), 0);

            fixture.EventsLogged.Clear();

        }

        [Fact]
        public void CounterWritesResult()
        {
            fixture.EventsLogged.Clear();

            var check = fixture.Logger.CountOperation("invocations", "times", false);

            Assert.Equal(check.Value(), 0);

            check.Increment();
            Assert.Equal(check.Value(), 1);

            check.Write();

            Assert.Equal(LogEventLevel.Information, fixture.EventSeen.Level);
            Assert.Equal("\"invocations\" count = 1 times", fixture.EventSeen.RenderMessage());

            fixture.EventsLogged.Clear();
        }

        [Fact]
        public void CounterWithCustomLevelWritesWithThatLevel()
        {
            fixture.EventsLogged.Clear();

            var check = fixture.Logger.CountOperation("invocations", "times", false, LogEventLevel.Debug);


            check.Write();

            Assert.Equal(LogEventLevel.Debug, fixture.EventSeen.Level);

            fixture.EventsLogged.Clear();
        }

        [Fact]
        public void CounterWritesDirectResultsToLogger()
        {
            fixture.EventsLogged.Clear();

            var check = fixture.Logger.CountOperation("invocations", "times", true);

            check.Increment();
            Assert.Equal("\"invocations\" count = 1 times", fixture.EventSeen.RenderMessage());

            check.Increment();
            Assert.Equal("\"invocations\" count = 2 times", fixture.EventSeen.RenderMessage());

            check.Decrement();
            Assert.Equal("\"invocations\" count = 1 times", fixture.EventSeen.RenderMessage());

            check.Reset();
            Assert.Equal("\"invocations\" count = 0 times", fixture.EventSeen.RenderMessage());

            fixture.EventsLogged.Clear();
        }

        [Fact]
        public void CounterWritesAtSpecifiedResolution()
        {
            fixture.EventsLogged.Clear();

            var check = fixture.Logger.CountOperation("invocations", "times", true, resolution: 3);

            for (var i = 0; i < 20; i++)
            {
                check.Increment();
            }

            Assert.Equal(6, fixture.EventsLogged.Count);

            Assert.Equal("\"invocations\" count = 18 times at 3 resolution", fixture.EventSeen.RenderMessage());

            fixture.EventsLogged.Clear();
        }


    }

}
