using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Options
{
	public interface IOptionView : IView
	{
		/// <summary>
		/// Raised when the user presses the option button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Sets the selected state of the option.
		/// </summary>
		/// <param name="mode"></param>
		void SetMode(ushort mode);
	}
}
