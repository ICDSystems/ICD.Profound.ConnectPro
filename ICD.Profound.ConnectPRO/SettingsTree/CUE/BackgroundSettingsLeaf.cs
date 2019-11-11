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
		public BackgroundSettingsLeaf()
		{
			Name = "Background";
			Icon = SettingsTreeIcons.ICON_BACKGROUNDS;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnBackgroundModeChanged = null;

			base.Dispose();
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

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			m_Theme = room == null ? null : room.Core.Originators.GetChild<ConnectProTheme>();
			Subscribe(m_Theme);
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			Unsubscribe(m_Theme);
			m_Theme = null;
		}

		#region Theme Callbacks

		/// <summary>
		/// Subscribe to the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Subscribe(ConnectProTheme theme)
		{
			if (theme == null)
				return;

			theme.OnCueBackgroundChanged += ThemeOnCueBackgroundChanged;
		}

		/// <summary>
		/// Unsubscribe from the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Unsubscribe(ConnectProTheme theme)
		{
			if (theme == null)
				return;

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
