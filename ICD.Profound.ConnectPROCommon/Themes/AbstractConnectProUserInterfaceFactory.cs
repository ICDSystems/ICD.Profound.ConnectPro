using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Connect.Themes.UserInterfaces;

namespace ICD.Profound.ConnectPROCommon.Themes
{
	public abstract class AbstractConnectProUserInterfaceFactory<TTheme, TUserInterface> : AbstractUserInterfaceFactory<TUserInterface>
		where TTheme : IConnectProTheme
		where TUserInterface : IUserInterface
	{
		/// <summary>
		/// Gets the theme.
		/// </summary>
		new protected TTheme Theme { get { return (TTheme)base.Theme; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		protected AbstractConnectProUserInterfaceFactory(TTheme theme)
			: base(theme)
		{
		}
	}
}
