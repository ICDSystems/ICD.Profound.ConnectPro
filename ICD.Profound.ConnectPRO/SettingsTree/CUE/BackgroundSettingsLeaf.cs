using System;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.Server.Osd;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes;

namespace ICD.Profound.ConnectPRO.SettingsTree.CUE
{
	public sealed class BackgroundSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Raised when the background mode changes.
		/// </summary>
		public event EventHandler OnBackgroundModeChanged;

		private ConnectProTheme m_Theme;

		#region Properties

		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public override bool Visible
		{
			get { return base.Visible && Room.Originators.GetInstancesRecursive<OsdPanelDevice>().Any(); }
		}

		/// <summary>
		/// Gets the background mode.
		/// </summary>
		public eCueBackgroundMode BackgroundMode { get { return m_Theme.CueBackground; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public BackgroundSettingsLeaf(IConnectProRoom room)
			: base(room)
		{
			Name = "Background";
			Icon = SettingsTreeIcons.ICON_BACKGROUNDS;

			m_Theme = Room.Core.Originators.GetChild<ConnectProTheme>();
			Subscribe(m_Theme);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnBackgroundModeChanged = null;

			base.Dispose();

			Unsubscribe(m_Theme);

			m_Theme = null;
		}

		/// <summary>
		/// Sets the CUE background mode.
		/// </summary>
		/// <param name="mode"></param>
		public void SetBackground(eCueBackgroundMode mode)
		{
			if (mode == m_Theme.CueBackground)
				return;

			m_Theme.CueBackground = mode;

			SetDirty(true);
		}

		#region Theme Callbacks

		/// <summary>
		/// Subscribe to the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Subscribe(ConnectProTheme theme)
		{
			theme.OnCueBackgroundChanged += ThemeOnCueBackgroundChanged;
		}

		/// <summary>
		/// Unsubscribe from the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Unsubscribe(ConnectProTheme theme)
		{
			theme.OnCueBackgroundChanged -= ThemeOnCueBackgroundChanged;
		}

		/// <summary>
		/// Called when the theme background mode changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ThemeOnCueBackgroundChanged(object sender, EventArgs eventArgs)
		{
			OnBackgroundModeChanged.Raise(this);
		}

		#endregion
	}
}
