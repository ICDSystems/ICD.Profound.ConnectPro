using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	public sealed class OsdWelcomePresenter : AbstractOsdPresenter<IOsdWelcomeView>, IOsdWelcomePresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdWelcomePresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) :
			base(nav, views, theme)
		{
		}

		protected override void Refresh(IOsdWelcomeView view)
		{
			base.Refresh(view);

			string name = Room == null ? string.Empty : Room.Name;
			view.SetRoomName(name);
		}
	}
}
