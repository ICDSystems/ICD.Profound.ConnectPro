using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.TouchFreeConferencing
{
	[ViewBinding(typeof(ISettingsCountdownTimerView))]
	public sealed partial class SettingsCountdownTimerView : AbstractUiView, ISettingsCountdownTimerView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsCountdownTimerView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		
	}
}