using System;
using OsmSharp.IO.Output;

namespace OsmSharp.Android.UI.IO.Output
{
	/// <summary>
	/// Writes the output to console.
	/// </summary>
	public class ConsoleOutputStream : IOutputStream
	{
		#region IOutputTextStream Members

		/// <summary>
		/// Writes text.
		/// </summary>
		/// <param name="text"></param>
		public void WriteLine(string text)
		{
			global::Android.Util.Log.Debug("Tag", text);
		}

		/// <summary>
		/// Writes text.
		/// </summary>
		/// <param name="text"></param>
		public void Write(string text)
		{
			global::Android.Util.Log.Debug("Tag", text);
		}

		private string _previous_progress_string;

		//private string _previous_key;

		/// <summary>
		/// Reports progress.
		/// </summary>
		/// <param name="progress"></param>
		/// <param name="key"></param>
		/// <param name="message"></param>
		public void ReportProgress(double progress, string key, string message)
		{
			string current_progress_string = message;//string.Format("{0}:{1}", key, message);

			
			global::Android.Util.Log.Debug("Tag", string.Format("{0} : {1}%",
			                                                    current_progress_string, System.Math.Round(progress * 100, 2).ToString().PadRight(6), key, message));
			_previous_progress_string = current_progress_string;
		}

		#endregion
	}
}

