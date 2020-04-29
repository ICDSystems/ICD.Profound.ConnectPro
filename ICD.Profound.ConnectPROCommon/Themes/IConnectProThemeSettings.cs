using ICD.Connect.Themes;

namespace ICD.Profound.ConnectPROCommon.Themes
{
	public interface IConnectProThemeSettings : IThemeSettings
	{
		string Logo { get; set; }
		string TvPresets { get; set; }
		string WebConferencingInstructions { get; set; }
		eCueBackgroundMode CueBackground { get; set; }
		bool CueMotion { get; set; }
	}
}