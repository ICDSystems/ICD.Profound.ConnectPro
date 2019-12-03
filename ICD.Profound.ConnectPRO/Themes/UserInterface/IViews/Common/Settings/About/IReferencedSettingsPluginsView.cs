namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.About
{
	public interface IReferencedSettingsPluginsView : IUiView
	{
		/// <summary>
		/// Sets the plugin label text.
		/// </summary>
		/// <param name="plugin"></param>
		void SetPluginLabel(string plugin);

		/// <summary>
		/// Sets the version label text.
		/// </summary>
		/// <param name="version"></param>
		void SetVersionLabel(string version);

		/// <summary>
		/// Sets the date label text.
		/// </summary>
		/// <param name="date"></param>
		void SetDateLabel(string date);
	}
}