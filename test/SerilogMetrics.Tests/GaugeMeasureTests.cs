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


using System.Collections.Generic;
using Serilog;
using Xunit;

namespace SerilogMetrics.Tests
{


    public class GaugeMeasureTests : IClassFixture<SerilogFixture>
    {

        SerilogFixture fixture;

        public GaugeMeasureTests(SerilogFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
		public void GaugeShouldReturnMeasure ()
		{
			var queue = new Queue<int>();
			var gauge = fixture.Logger.GaugeOperation("queue", "item(s)", () => queue.Count);

			gauge.Write ();
			Assert.Equal ("\"queue\" value = 0 item(s)", fixture.EventSeen.RenderMessage());

			queue.Enqueue (1);
			queue.Enqueue (1);

			gauge.Write ();
			Assert.Equal ("\"queue\" value = 2 item(s)", fixture.EventSeen.RenderMessage());

			queue.Dequeue ();

			gauge.Write ();
			Assert.Equal ("\"queue\" value = 1 item(s)", fixture.EventSeen.RenderMessage());

			queue.Clear ();

			gauge.Write ();
			Assert.Equal ("\"queue\" value = 0 item(s)", fixture.EventSeen.RenderMessage());

		}
	}
	
}
