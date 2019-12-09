using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;

namespace ICD.Profound.ConnectPRO.Routing.Endpoints.Sources
{
	public enum eControlOverride
	{
		Default,
		CableTv,
		Vtc,
		Atc,
		WebConference
	}

	public enum eConferenceOverride
	{
		/// <summary>
		/// Default behaviour - Privacy Mute and Camera features are available if
		/// the source is a conferencing device.
		/// </summary>
		None = 0,

		/// <summary>
		/// Hides the Privacy Mute and Camera features even if the source is a
		/// conferencing device.
		/// </summary>
		Hide = 1,

		/// <summary>
		/// Shows the Privacy Mute and Camera features even if the source is NOT
		/// a conferencing device. This is especially useful for web conferencing
		/// devices (i.e. a user plugging in a Laptop).
		/// </summary>
		Show = 2
	}

	public sealed class ConnectProSource : AbstractSource<ConnectProSourceSettings>
	{
		/// <summary>
		/// Gets/sets the icon serial.
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// Gets/sets if this source can be shared.
		/// </summary>
		public bool Share { get; set; }

		/// <summary>
		/// Gets/sets the name of this source to be displayed on the Cue.
		/// Only unique names are displayed
		/// </summary>
		public string CueName { get; set; }

		/// <summary>
		/// Gets/sets the type of control to show when selecting the source in the UI.
		/// </summary>
		public eControlOverride ControlOverride { get; set; }

		/// <summary>
		/// Overrides the availability of privacy mute and camera features while the source is routed.
		/// </summary>
		public eConferenceOverride ConferenceOverride { get; set; }

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Icon = null;
			Share = false;
			CueName = null;
			ControlOverride = eControlOverride.Default;
			ConferenceOverride = eConferenceOverride.None;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(ConnectProSourceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Icon = Icon;
			settings.Share = Share;
			settings.CueName = CueName;
			settings.ControlOverride = ControlOverride;
			settings.ConferenceOverride = ConferenceOverride;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(ConnectProSourceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			Icon = settings.Icon;
			Share = settings.Share;
			CueName = settings.CueName;
			ControlOverride = settings.ControlOverride;
			ConferenceOverride = settings.ConferenceOverride;
		}
	}
}
