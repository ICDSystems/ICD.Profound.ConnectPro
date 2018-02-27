using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	public sealed partial class DisabledAlertView : AbstractView, IDisabledAlertView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public DisabledAlertView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}