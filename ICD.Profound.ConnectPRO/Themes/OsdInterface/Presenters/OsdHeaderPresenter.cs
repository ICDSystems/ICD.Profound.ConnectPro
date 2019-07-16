using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	[PresenterBinding(typeof(IOsdHeaderPresenter))]
	public sealed class OsdHeaderPresenter : AbstractOsdPresenter<IOsdHeaderView>, IOsdHeaderPresenter
	{
		public OsdHeaderPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		protected override void Refresh(IOsdHeaderView view)
		{
			base.Refresh(view);

			view.SetRoomName(Room == null ? string.Empty : Room.Name);
			view.SetTimeLabel(ConnectProDateFormatting.ShortTime);
		}
	}
}
