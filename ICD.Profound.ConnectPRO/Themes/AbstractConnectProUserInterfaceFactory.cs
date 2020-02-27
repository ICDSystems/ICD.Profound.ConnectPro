using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Connect.Themes.UserInterfaces;

namespace ICD.Profound.ConnectPRO.Themes
{
	public abstract class AbstractConnectProUserInterfaceFactory<TUserInterface> : AbstractUserInterfaceFactory<TUserInterface>
		where TUserInterface : IUserInterface
	{
		/// <summary>
		/// Gets the theme.
		/// </summary>
		new protected ConnectProTheme Theme { get { return base.Theme as ConnectProTheme; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		protected AbstractConnectProUserInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}
	}
}
