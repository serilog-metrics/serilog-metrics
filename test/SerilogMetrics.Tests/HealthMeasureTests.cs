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
using Serilog;
using System.Reactive.Linq;
using Xunit;


namespace SerilogMetrics.Tests
{


    public class HealthMeasureTests : IClassFixture<SerilogFixture>
    {

        SerilogFixture fixture;

        public HealthMeasureTests(SerilogFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void HealthyCheckResultShouldReportInformation()
        {
            var check = fixture.Logger.HealthCheck("test-healthy", () => new HealthCheckResult());

            check.Write();

            Assert.Equal(fixture.EventSeen.RenderMessage(), "Health check \"test-healthy\" result is \"successful\".");
            Assert.True(fixture.EventSeen.Level == LogEventLevel.Information);
        }

        [Fact]
        public void HealthyCheckWithCustomLevelResultShouldReportCustomLevel()
        {
            var check = fixture.Logger.HealthCheck("test-healthy", () => new HealthCheckResult(), LogEventLevel.Verbose);

            check.Write();

            Assert.Equal(fixture.EventSeen.RenderMessage(), "Health check \"test-healthy\" result is \"successful\".");
            Assert.True(fixture.EventSeen.Level == LogEventLevel.Verbose);
        }

        [Fact]
        public void UnHealthyCheckResultShouldReportWarning()
        {

            var check = fixture.Logger.HealthCheck("test-unhealthy", () => new HealthCheckResult("something was wrong", new ArgumentException()));

            check.Write();

            Assert.True(fixture.EventSeen.RenderMessage() == "Health check \"test-unhealthy\" result is \"something was wrong\".");
            Assert.True(fixture.EventSeen.Level == LogEventLevel.Warning);
        }

        [Fact]
        public void UnHealthyCheckWithCustomLevelResultShouldReportCustomLevel()
        {

            var check = fixture.Logger.HealthCheck("test-unhealthy", () => new HealthCheckResult("something was wrong", new ArgumentException()), LogEventLevel.Information, LogEventLevel.Fatal);

            check.Write();

            Assert.True(fixture.EventSeen.RenderMessage() == "Health check \"test-unhealthy\" result is \"something was wrong\".");
            Assert.True(fixture.EventSeen.Level == LogEventLevel.Fatal);
        }

        [Fact]
        public void HealthyCheckWithExceptionMustBeCaptured()
        {

            var check = fixture.Logger.HealthCheck("test-exception", () =>
            {
                // Something goes wrong here
                throw new ArgumentException();
            });

            check.Write();

            Assert.True(fixture.EventSeen.RenderMessage() == "Health check \"test-exception\" result is \"Unable to execute the health check named 'test-exception'. See inner exception for more details.\".");
            Assert.NotNull(fixture.EventSeen.Exception);
            Assert.IsType(typeof(ArgumentException), fixture.EventSeen.Exception);
            Assert.True(fixture.EventSeen.Level == LogEventLevel.Error);
        }

    }
}

