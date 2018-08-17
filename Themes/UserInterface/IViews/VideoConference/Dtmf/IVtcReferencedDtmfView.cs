using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Dtmf
{
	public interface IVtcReferencedDtmfView : IView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Sets the label text.
		/// </summary>
		/// <param name="label"></param>
		void SetLabel(string label);

		/// <summary>
		/// Sets the selected state of the button.
		/// </summary>
		/// <param name="selected"></param>
		void SetSelected(bool selected);
	}
}