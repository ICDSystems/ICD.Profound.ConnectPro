using ICD.Common.Utils;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class MenuRouteSummaryPresenter : AbstractUiPresenter<IMenuRouteSummaryView>, IMenuRouteSummaryPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		public MenuRouteSummaryPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IMenuRouteSummaryView view)
		{
			base.Refresh(view);
			
			if (Room == null)
				return;

			m_RefreshSection.Enter();
			try
			{
				// todo - know the logic, don't know the applicable api surface well enough
				// get rooms in combined room
				// foreach room
				// get displays in those rooms that are also in combined room
				// foreach display
				// get source routed to that display


			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}
	}
}
