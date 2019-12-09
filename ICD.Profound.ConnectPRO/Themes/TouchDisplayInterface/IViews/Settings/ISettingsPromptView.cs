namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings
{
	public interface ISettingsPromptView : ITouchDisplayView
	{
		/// <summary>
		/// Sets the path to the illustration graphic.
		/// </summary>
		/// <param name="image"></param>
		void SetImage(string image);

		/// <summary>
		/// Sets the text for the help label.
		/// </summary>
		/// <param name="help"></param>
		void SetHelpText(string help);

		/// <summary>
		/// Sets the text for the title label.
		/// </summary>
		/// <param name="title"></param>
		void SetTitle(string title);
	}
}
