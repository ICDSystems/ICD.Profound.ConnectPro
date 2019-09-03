using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	[ViewBinding(typeof(ISettingsPromptView))]
	public sealed partial class SettingsPromptView : AbstractUiView, ISettingsPromptView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsPromptView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Sets the path to the illustration graphic.
		/// </summary>
		/// <param name="path"></param>
		public void SetImagePath(string path)
		{
			m_Image.SetImageUrl(path);
		}

		/// <summary>
		/// Sets the text for the help label.
		/// </summary>
		/// <param name="help"></param>
		public void SetHelpText(string help)
		{
			m_HelpLabel.SetLabelText(help);
		}
	}
}
