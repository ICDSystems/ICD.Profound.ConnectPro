using ICD.Connect.Lighting.RoomInterface;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
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

		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			IsAvailable = room != null && room.Originators.GetInstanceRecursive<ILightingRoomInterfaceDevice>() != null;
		}

		/// <summary>
		/// This method will get called when the button is pressed - also called by the FloatingActionListButtonPresenter if this is the only icon
		/// </summary>
		public override void HandleButtonPressed()
		{
			if (m_ActionPresenter.PresetsAvailable)
			{
				base.HandleButtonPressed();
				return;
			}

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show("Room has no lighting presets available",
			                10 * 1000,
			                GenericAlertPresenterButton.Dismiss);
		}
	}
}
