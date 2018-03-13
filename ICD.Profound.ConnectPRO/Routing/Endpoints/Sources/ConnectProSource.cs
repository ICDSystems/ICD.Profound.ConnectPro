using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Core;

namespace ICD.Profound.ConnectPRO.Routing.Endpoints.Sources
{
	public sealed class ConnectProSource : AbstractSource<ConnectProSourceSettings>
	{
		/// <summary>
		/// Gets/sets the icon serial.
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// Gets/sets the text describing this source.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets if this source can be shared.
		/// </summary>
		public bool Share { get; set; }

		/// <summary>
		/// Gets/sets if the source should be hidden from the source list.
		/// </summary>
		public bool Hide { get; set; }

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Icon = null;
			Description = null;
			Share = false;
			Hide = false;
		}

		protected override void CopySettingsFinal(ConnectProSourceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Icon = Icon;
			settings.Description = Description;
			settings.Share = Share;
			settings.Hide = Hide;
		}

		protected override void ApplySettingsFinal(ConnectProSourceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			Icon = settings.Icon;
			Description = settings.Description;
			Share = settings.Share;
			Hide = Hide;
		}
	}
}
