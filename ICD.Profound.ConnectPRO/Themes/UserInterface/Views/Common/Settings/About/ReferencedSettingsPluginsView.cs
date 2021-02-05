using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.About;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.About
{
	[ViewBinding(typeof(IReferencedSettingsPluginsView))]
	public sealed partial class ReferencedSettingsPluginsView : AbstractComponentView, IReferencedSettingsPluginsView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ReferencedSettingsPluginsView(ISigInputOutput panel, IConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		/// <summary>
		/// Sets the plugin label text.
		/// </summary>
		/// <param name="plugin"></param>
		public void SetPluginLabel(string plugin)
		{
			m_PluginLabel.SetLabelText(plugin);
		}

		/// <summary>
		/// Sets the version label text.
		/// </summary>
		/// <param name="version"></param>
		public void SetVersionLabel(string version)
		{
			m_VersionLabel.SetLabelText(version);
		}

		/// <summary>
		/// Sets the date label text.
		/// </summary>
		/// <param name="date"></param>
		public void SetDateLabel(string date)
		{
			m_DateLabel.SetLabelText(date);
		}
	}
}
