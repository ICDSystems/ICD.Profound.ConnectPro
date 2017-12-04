using ICD.Connect.Panels;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups
{
	public abstract class AbstractPopupView : AbstractView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractPopupView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
