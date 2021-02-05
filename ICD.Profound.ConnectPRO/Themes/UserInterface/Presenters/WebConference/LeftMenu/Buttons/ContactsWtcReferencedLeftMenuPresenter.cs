using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(IContactsWtcReferencedLeftMenuPresenter))]
	public sealed class ContactsWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                             IContactsWtcReferencedLeftMenuPresenter
	{
		private readonly IWtcContactListPresenter m_ContactListPresenter;

		/// <summary>
		/// Gets the zoom traditional control for call out.
		/// </summary>
		private ZoomRoomTraditionalConferenceControl TraditionalControl
		{
			get { return GetTraditionalConferenceControl(ActiveConferenceControl); }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ContactsWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                              IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_ContactListPresenter = Navigation.LazyLoadPresenter<IWtcContactListPresenter>();
			Subscribe(m_ContactListPresenter);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_ContactListPresenter);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedLeftMenuView view)
		{
			Icon = "list";
			Label = "Contacts";
			State = null;
			Enabled = TraditionalControl == null || TraditionalControl.GetActiveConference() == null;
			Selected = m_ContactListPresenter.IsViewVisible;

			base.Refresh(view);
		}

		public override void HideSubpages()
		{
			m_ContactListPresenter.ShowView(false);
		}

		/// <summary>
		/// Gets the zoom traditional control for call out from the given conference control.
		/// </summary>
		[CanBeNull]
		private static ZoomRoomTraditionalConferenceControl GetTraditionalConferenceControl(
			[CanBeNull] IWebConferenceDeviceControl control)
		{
			if (control == null)
				return null;

			ZoomRoom device = control.Parent as ZoomRoom;
			return device == null ? null : device.Controls.GetControl<ZoomRoomTraditionalConferenceControl>();
		}

		#region Presenter Callbacks

		private void Subscribe(IWtcContactListPresenter contactListPresenter)
		{
			contactListPresenter.OnViewVisibilityChanged += ContactListPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(IWtcContactListPresenter contactListPresenter)
		{
			contactListPresenter.OnViewVisibilityChanged -= ContactListPresenterOnViewVisibilityChanged;
		}

		private void ContactListPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (IWebConference conference in control.GetConferences())
				Subscribe(conference);

			ZoomRoomTraditionalConferenceControl callOut = GetTraditionalConferenceControl(control);
			if (callOut == null)
				return;

			callOut.OnConferenceAdded += TraditionalControlOnConferenceAdded;
			callOut.OnConferenceRemoved += TraditionalControlOnConferenceRemoved;

			foreach (ITraditionalConference conference in callOut.GetConferences())
				Subscribe(conference);
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (IWebConference conference in control.GetConferences())
				Unsubscribe(conference);

			ZoomRoomTraditionalConferenceControl callOut = GetTraditionalConferenceControl(control);
			if (callOut == null)
				return;

			callOut.OnConferenceAdded -= TraditionalControlOnConferenceAdded;
			callOut.OnConferenceRemoved -= TraditionalControlOnConferenceRemoved;

			foreach (ITraditionalConference conference in callOut.GetConferences())
				Unsubscribe(conference);
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			RefreshIfVisible();
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			RefreshIfVisible();
		}

		private void TraditionalControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			RefreshIfVisible();
		}

		private void TraditionalControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnParticipantRemoved;
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			conference.OnParticipantAdded -= ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved -= ConferenceOnParticipantRemoved;
			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnParticipantAdded(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnParticipantRemoved(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			Navigation.NavigateTo<IWtcContactListPresenter>();
		}
	}
}
