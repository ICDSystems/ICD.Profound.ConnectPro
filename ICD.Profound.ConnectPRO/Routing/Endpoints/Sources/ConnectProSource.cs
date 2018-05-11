using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;

namespace ICD.Profound.ConnectPRO.Routing.Endpoints.Sources
{
	public enum eControlOverride
	{
		Default,
		CableTv,
		Vtc,
		WebConference
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
		/// Gets/sets the type of control to show when selecting the source in the UI.
		/// </summary>
		public eControlOverride ControlOverride { get; set; }

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Icon = null;
			Share = false;
			ControlOverride = eControlOverride.Default;
		}

		protected override void CopySettingsFinal(ConnectProSourceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Icon = Icon;
			settings.Share = Share;
			settings.ControlOverride = ControlOverride;
		}

		protected override void ApplySettingsFinal(ConnectProSourceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			Icon = settings.Icon;
			Share = settings.Share;
			ControlOverride = settings.ControlOverride;
		}
	}
}
