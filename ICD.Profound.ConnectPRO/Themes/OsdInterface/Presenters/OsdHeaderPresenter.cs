using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	public sealed class OsdHeaderPresenter : AbstractOsdPresenter<IOsdHeaderView>, IOsdHeaderPresenter
	{
		public OsdHeaderPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		protected override void Refresh(IOsdHeaderView view)
		{
			base.Refresh(view);

			view.SetRoomName(Room != null ? Room.Name : string.Empty);
		}
	}
}
