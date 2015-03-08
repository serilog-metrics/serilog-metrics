using System;
using Serilog;
using System.Diagnostics;
using System.Threading;

namespace SerilogMetrics
{
	/// <summary>
	/// I progress logger.
	/// </summary>
	public interface IProgressLogger: IDisposable{

	}

	/// <summary>
	/// Progress logger.
	/// </summary>
	public class ProgressLogger: IProgressLogger
	{
		private  ILogger _logger;
		private object _id;
		private readonly Stopwatch _stopwatch;
		private readonly string _name;
		private CancellationTokenSource _tokenSource;

		/// <summary>
		/// Initializes a new instance of the <see cref="SerilogMetrics.ProgressLogger"/> class.
		/// </summary>
		/// <param name="logger">Logger.</param>
		/// <param name = "id"></param>
		/// <param name="name">Name.</param>
		/// <param name = "tokenSource"></param>
		public ProgressLogger (ILogger logger, object id, string name, CancellationTokenSource tokenSource=null)
		{

			_stopwatch = Stopwatch.StartNew ();
			_name = name;
			_id = id;
			_tokenSource = tokenSource ?? new CancellationTokenSource();

			_logger = logger
				.ForContext("ProgressName",_name)
				.ForContext("ProgressId",_id)
				.ForContext("ProgressStartUtc", DateTime.UtcNow) ;

			_logger.Information ("Started the progress {progressName}", _name);


		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="SerilogMetrics.ProgressLogger"/> is finished.
		/// </summary>
		/// <value><c>true</c> if finished; otherwise, <c>false</c>.</value>
		public bool Finished { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this instance is canceled.
		/// </summary>
		/// <value><c>true</c> if this instance is canceled; otherwise, <c>false</c>.</value>
		public bool IsCanceled
		{
			get
			{
				return Token.IsCancellationRequested;
			}
		}

		/// <summary>
		/// Gets the token.
		/// </summary>
		/// <value>The token.</value>
		public CancellationToken Token
		{
			get { return _tokenSource.Token; }
		}

		/// <summary>
		/// Fail the specified ex, messageTemplate and propertyValues.
		/// </summary>
		/// <param name="ex">Ex.</param>
		/// <param name="messageTemplate">Message template.</param>
		/// <param name="propertyValues">Property values.</param>
		public virtual void Fail(Exception ex, string messageTemplate, params object[] propertyValues){
			_logger.Error (ex, messageTemplate, propertyValues);
		}

		/// <summary>
		/// Finish this instance.
		/// </summary>
		public virtual void Finish(){

			_stopwatch.Stop ();
			Finished = true;


			_logger.Information ("Finished the progress {progressName} in {TimedOperationElapsed} ({TimedOperationElapsedInMs} ms)", _name, _stopwatch.Elapsed, _stopwatch.ElapsedMilliseconds);
		}

		/// <summary>
		/// Determines whether this instance cancel  .
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		public virtual void Cancel(){

		}

		/// <summary>
		/// Progress this instance.
		/// </summary>
		public virtual void Progress(){

		}


		#region IDisposable implementation
		/// <summary>
		/// Releases all resource used by the <see cref="SerilogMetrics.ProgressLogger"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="SerilogMetrics.ProgressLogger"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="SerilogMetrics.ProgressLogger"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="SerilogMetrics.ProgressLogger"/>
		/// so the garbage collector can reclaim the memory that the <see cref="SerilogMetrics.ProgressLogger"/> was occupying.</remarks>
		public virtual void Dispose ()
		{
			if (!Finished) {
				Fail (null, "finished");
			}
		}
		#endregion

	}
}

