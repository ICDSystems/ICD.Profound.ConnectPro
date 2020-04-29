using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Settings
{
	[ViewBinding(typeof(ISettingsPromptView))]
	public sealed partial class SettingsPromptView : AbstractTouchDisplayView, ISettingsPromptView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsPromptView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Sets the illustration graphic.
		/// </summary>
		/// <param name="image"></param>
		public void SetImage(string image)
		{
			m_Image.SetIcon(image);
		}

		/// <summary>
		/// Sets the text for the help label.
		/// </summary>
		/// <param name="help"></param>
		public void SetHelpText(string help)
		{
			m_HelpLabel.SetLabelText(help);
		}

		/// <summary>
		/// Sets the text for the title label.
		/// </summary>
		/// <param name="title"></param>
		public void SetTitle(string title)
		{
			m_Title.SetLabelText(title);
		}
	}
}
