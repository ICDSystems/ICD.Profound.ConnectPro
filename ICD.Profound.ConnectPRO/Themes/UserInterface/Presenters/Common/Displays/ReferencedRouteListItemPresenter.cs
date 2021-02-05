using ICD.Common.Utils;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	[PresenterBinding(typeof(IReferencedRouteListItemPresenter))]
	public sealed class ReferencedRouteListItemPresenter : AbstractUiComponentPresenter<IReferencedRouteListItemView>, IReferencedRouteListItemPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		
		public RouteListItem Model { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedRouteListItemPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                        IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedRouteListItemView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string roomLabel = Model.Room == null ? string.Empty : Model.Room.GetName(true);
				string displayLabel = Model.Destination == null ? string.Empty : Model.Destination.GetName(true);
				string sourceLabel = Model.Source == null ? string.Empty : Model.Source.GetName(true);

				view.SetRoomLabelText(roomLabel);
				view.SetDisplayLabelText(displayLabel);
				view.SetSourceLabelText(sourceLabel);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}
	}
}
