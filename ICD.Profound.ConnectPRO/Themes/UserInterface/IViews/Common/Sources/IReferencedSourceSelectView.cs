using System;
using ICD.Profound.ConnectPROCommon.Routing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources
{
	public interface IReferencedSourceSelectView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		void SetColor(eSourceColor color);

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		void SetIcon(string icon);

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		void SetLine1Text(string text);

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		void SetLine2Text(string text);

		/// <summary>
		/// Sets the text for the feedback label.
		/// </summary>
		/// <param name="text"></param>
		void SetFeedbackText(string text);

		/// <summary>
		/// Sets the routed state for the source.
		/// </summary>
		/// <param name="sourceState"></param>
		void SetRoutedState(eSourceState sourceState);
	}
}
