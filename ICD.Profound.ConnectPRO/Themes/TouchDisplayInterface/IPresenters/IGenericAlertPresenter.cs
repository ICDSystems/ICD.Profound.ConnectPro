using System;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters
{
	public interface IGenericAlertPresenter : ITouchDisplayPresenter<IGenericAlertView>
	{
		/// <summary>
		/// Set the message of the presenter and show it.
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="buttons"></param>
		void Show(string message, params GenericAlertPresenterButton[] buttons);

		/// <summary>
		/// Set the message of the presenter and show it for the given time.
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="timeout">Time in milliseconds to show the popup</param>
		/// <param name="buttons"></param>
		void Show(string message, long timeout, params GenericAlertPresenterButton[] buttons);
	}

	public sealed class GenericAlertPresenterButton
	{
		private static readonly GenericAlertPresenterButton s_Dismiss =
			new GenericAlertPresenterButton
			{
				Label = "Dismiss",
				Enabled = true,
				Visible = true,
				UseDismissColor = true,
				PressCallback = p => { }
			};

		public static GenericAlertPresenterButton Dismiss { get { return s_Dismiss; } }

		public string Label { get; set; }
		public bool Enabled { get; set; }
		public bool Visible { get; set; }
		public bool UseDismissColor { get; set; }
		public Action<IGenericAlertPresenter> PressCallback { get; set; }
	}
}