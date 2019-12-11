using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Settings.Cores;
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

		[CanBeNull]
		private ConnectProTheme m_Theme;

		#region Properties

		[CanBeNull]
		private ConnectProTheme Theme
		{
			get { return m_Theme; }
			set
			{
				if (value == m_Theme)
					return;

				Unsubscribe(m_Theme);
				m_Theme = value;
				Subscribe(m_Theme);

				UpdateBackgroundMode();
			}
		}

		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public override bool Visible
		{
			get
			{
				return base.Visible &&
				       Room != null &&
				       (Room.Originators.GetInstancesRecursive<OsdPanelDevice>().Any() 
				        || Room.Originators.GetInstancesRecursive<VibeBoard>().Any());
			}
		}

		/// <summary>
		/// Gets the background mode.
		/// </summary>
		public eCueBackgroundMode BackgroundMode
		{
			get { return Theme == null ? default(eCueBackgroundMode) : Theme.CueBackground; }
			set
			{
				if (Theme == null || value == Theme.CueBackground)
					return;

				Theme.CueBackground = value;

				SetDirty(true);

				OnBackgroundModeChanged.Raise(this);
			}
		}

		/// <summary>
		/// Gets the background motion.
		/// </summary>
		public bool BackgroundMotion { get { return m_Theme.CueMotion; } }

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

		#region Private Methods

		private void UpdateBackgroundMode()
		{
			BackgroundMode = Theme == null ? default(eCueBackgroundMode) : Theme.CueBackground;
		}

		private void UpdateTheme()
		{
			Theme = Room == null ? null : Room.Core.Originators.GetChildren<ConnectProTheme>().FirstOrDefault();
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Sets whether to use video backgrounds (true) or image backgrounds (false).
		/// </summary>
		/// <param name="video"></param>
		public void SetBackgroundMotion(bool motion)
		{
			m_Theme.CueMotion = motion;
		}

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			// The theme may not have loaded yet
			ICore core = room.Core;
			core.Originators.OnChildrenChanged += OriginatorsOnChildrenChanged;

			UpdateTheme();
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			// The theme may not have loaded yet
			ICore core = room.Core;
			core.Originators.OnChildrenChanged -= OriginatorsOnChildrenChanged;

			UpdateTheme();
		}

		private void OriginatorsOnChildrenChanged(object sender, EventArgs e)
		{
			UpdateTheme();
		}

		#endregion

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
			UpdateBackgroundMode();
		}

		#endregion
	}
}
