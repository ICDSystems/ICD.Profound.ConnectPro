using System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;

namespace ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources
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

	[Flags]
	public enum eSourceAppearance
	{
		None,

		/// <summary>
		/// The source appears on pages for routing.
		/// </summary>
		Routing,

		/// <summary>
		/// The source appears on pages for presentation
		/// </summary>
		Presentation
	}

	public sealed class ConnectProSource : AbstractSource<ConnectProSourceSettings>
	{
		/// <summary>
		/// Gets/sets the icon serial.
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// Gets/sets the source appearance mask.
		/// </summary>
		public eSourceAppearance Appearance { get; set; }

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
		/// Returns the CueName, or the Icon string for cue
		/// Allows using CueName while preserving backwards compataibility
		/// </summary>
		public string CueNameOrIcon { get { return string.IsNullOrEmpty(CueName) ? Icon : CueName ; } }

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Icon = null;
			Appearance = eSourceAppearance.None;
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
			settings.Appearance = Appearance;
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
			Appearance = settings.Appearance;
			CueName = settings.CueName;
			ControlOverride = settings.ControlOverride;
			ConferenceOverride = settings.ConferenceOverride;
		}
	}
}
