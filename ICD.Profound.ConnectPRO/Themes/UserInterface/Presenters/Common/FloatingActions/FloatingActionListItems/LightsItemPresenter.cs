using ICD.Connect.Lighting.RoomInterface;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Lighting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions.FloatingActionListItems;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions.FloatingActionListItems
{
	public sealed class LightsItemPresenter : AbstractFloatingActionListItemPresenter<ILightsItemView>
	{
		private const string LABEL = "Lights";
		private const string ICON = "lights";

		private readonly ILightingPresenter m_ActionPresenter;

		/// <summary>
		/// This is the presenter for the page this list item shows
		/// </summary>
		protected override IUiPresenter ActionPresenter { get { return m_ActionPresenter; } }

		public override string Label { get { return LABEL; } }

		public override string Icon { get { return ICON; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public LightsItemPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_ActionPresenter = Navigation.LazyLoadPresenter<ILightingPresenter>();
		}

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;


			ILightingRoomInterfaceDevice lightingInterface = room.Originators.GetInstanceRecursive<ILightingRoomInterfaceDevice>();
			IsAvailable = lightingInterface != null;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			IsAvailable = false;
		}
	}
}
