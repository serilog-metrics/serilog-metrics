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

namespace SerilogMetrics
{
	/// <summary>
	/// Counter measure.
	/// </summary>
    public class CounterMeasure : ICounterMeasure
    {
        readonly ILogger _logger;
        readonly string _name;
        readonly string _counts;
        readonly LogEventLevel _level;
        readonly string _template;
        readonly bool _directWrite;
        readonly AtomicLong _value;
        readonly AtomicLong _runValue;
        readonly Func<long, long, bool> _writeRule; 

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterMeasure"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="name">The name.</param>
        /// <param name="counts">The counts.</param>
        /// <param name="level">The level.</param>
        /// <param name="template">The template.</param>
        /// <param name="directWrite">if set to <c>true</c> then directly write to the log.</param>
        /// <param name="writeRule">Func to determine when to write to logs based on the result of the Func</param>
        public CounterMeasure(ILogger logger, string name, string counts, LogEventLevel level, string template, bool directWrite = false, Func<long, long, bool> writeRule = null)
        {
            _logger = logger;
            _name = name;
            _counts = counts;
            _level = level;
            _template = template;
            _directWrite = directWrite;
            _value = new AtomicLong();
            _runValue = new AtomicLong();
            _writeRule = writeRule ?? ((v, c) => false);
        }

		/// <summary>
		/// Increments the counter.
		/// </summary>
        public virtual void Increment()
        {
            _value.Increment();
		    
            _runValue.Increment();

		    if ((_directWrite) || _writeRule(_value.Get(), _runValue.Get()))
		        Write();
        }

		/// <summary>
		/// Decrements the counter.
		/// </summary>
        public virtual void Decrement()
        {
            _value.Decrement();

            _runValue.Increment();

            if ((_directWrite) || _writeRule(_value.Get(), _runValue.Get()))
                Write();

        }

		/// <summary>
		/// Resets the counter back to zero.
		/// </summary>
        public virtual void Reset()
        {
            _value.Set(0);

            if ((_directWrite) || _writeRule(_value.Get(), _runValue.Get()))
                Write();
        }

		/// <summary>
		/// Write the measurement data to the log system.
		/// </summary>
		public virtual void Write()
        {
            var value = _value.Get();
            _logger.Write(_level, _template, _name, value, _counts);
        }

		/// <summary>
		/// Retrieves the current value.
		/// </summary>
		public virtual long Value(){

			return _value.Get ();
		}
    }
}