namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings
{
	public interface ISettingsPromptView : IUiView
	{
		/// <summary>
		/// Sets the path to the illustration graphic.
		/// </summary>
		/// <param name="path"></param>
		void SetImagePath(string path);

		/// <summary>
		/// Sets the text for the help label.
		/// </summary>
		/// <param name="help"></param>
		void SetHelpText(string help);
	}
}
