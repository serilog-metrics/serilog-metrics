using System;
using Serilog;

namespace SerilogMetrics
{
	public class ProgressLogger
	{
		private readonly ILogger _logger;
		private readonly string _name;

		public ProgressLogger (ILogger logger, string name)
		{
			_logger = logger;
			_name = name;
		}



		public void Fail(){

		}

		public void Finish(){

		}

		public void Progress(){

		}


	}
}

