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
using System.Collections.Generic;
using Serilog;
using Xunit;

namespace SerilogMetrics.Tests
{

    public class MeterMeasureTests : IClassFixture<SerilogFixture>
    {

        SerilogFixture fixture;

        public MeterMeasureTests(SerilogFixture fixture)
        {
            this.fixture = fixture;
        }


        [Fact]
        public void MeterShouldWriteOutput()
        {
            var meter = fixture.Logger.MeterOperation("server load", "requests", TimeUnit.Seconds);

            meter.Mark();

            meter.Write();

            Assert.Contains("\"server load\" count = 1, ", fixture.EventSeen.RenderMessage());


            System.Threading.Thread.Sleep(2000);

            meter.Mark(2);

            meter.Write();

            Assert.Contains("\"server load\" count = 3, ", fixture.EventSeen.RenderMessage());

            //Wait a minute
            System.Threading.Thread.Sleep(60000);

            meter.Mark(2);

            meter.Write();

            Assert.Contains("\"server load\" count = 5, ", fixture.EventSeen.RenderMessage());

            Assert.Equal(5, meter.Count);
        }
    }

}
