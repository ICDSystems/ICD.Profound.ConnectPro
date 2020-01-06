using ICD.Connect.Lighting.RoomInterface;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Lighting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions.FloatingActionListItems
{
	public sealed class LightsListItem : AbstractFloatingActionListItem
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
		public LightsListItem(INavigationController navigation): base(navigation)
		{
			m_ActionPresenter = Navigation.LazyLoadPresenter<ILightingPresenter>();
			SubscribeActionPresenter(m_ActionPresenter);
		}

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		public override void Subscribe(IConnectProRoom room)
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
		public override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			IsAvailable = false;
		}
	}
}
