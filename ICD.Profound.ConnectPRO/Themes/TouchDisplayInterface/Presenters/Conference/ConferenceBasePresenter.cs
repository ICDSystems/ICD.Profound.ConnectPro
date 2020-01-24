using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.Conferencing.Zoom.Controls;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Connect.Devices.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Notifications;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IConferenceBasePresenter))]
	public sealed class ConferenceBasePresenter : AbstractPopupPresenter<IConferenceBaseView>, IConferenceBasePresenter
	{
		private const string DISCONNECTING_TEXT = "Disconnecting... Audio and video may still be live.";
		private const string CONNECTING_TEXT = "Please wait. Your conference is being connected...";

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly List<IConferencePresenter> m_ConferencePresenters;
		private readonly Dictionary<HeaderButtonModel, ITouchDisplayPresenter> m_PresenterButtons;
		private readonly List<HeaderButtonModel> m_InCallButtons;
		private readonly List<HeaderButtonModel> m_OutOfCallButtons;
		private HeaderButtonModel m_HideCameraButton;
		private HeaderButtonModel m_EndConferenceButton;
		private IHeaderPresenter m_Header;
		
		private IConferenceDeviceControl m_SubscribedConferenceControl;
		private bool m_IsInCall;

		public IConferenceDeviceControl ActiveConferenceControl
		{
			get { return m_SubscribedConferenceControl; }
			set
			{
				if (m_SubscribedConferenceControl == value)
					return;

				Unsubscribe(m_SubscribedConferenceControl);
				m_SubscribedConferenceControl = value;
				Subscribe(m_SubscribedConferenceControl);
				
				foreach (var presenter in m_ConferencePresenters)
					presenter.ActiveConferenceControl = m_SubscribedConferenceControl;

				if (m_SubscribedConferenceControl == null)
					ShowView(false);
			}
		}

		private bool IsInCall
		{
			get { return m_IsInCall; }
			set
			{
				if (value == m_IsInCall)
					return;

				m_IsInCall = value;

				if (m_IsInCall)
					ShowView(true);

				ShowDefaultPresenter();
			}
		}

		public ConferenceBasePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			m_RefreshSection = new SafeCriticalSection();
			m_ConferencePresenters = Navigation.LazyLoadPresenters<IConferencePresenter>().ToList();
			foreach (var presenter in m_ConferencePresenters)
				Subscribe(presenter);

			m_PresenterButtons = new Dictionary<HeaderButtonModel, ITouchDisplayPresenter>();
			m_InCallButtons = new List<HeaderButtonModel>();
			m_OutOfCallButtons = new List<HeaderButtonModel>();
			InitHeaderButtons();
		}

		public override void Dispose()
		{
			foreach (var presenter in m_ConferencePresenters)
				Unsubscribe(presenter);

			base.Dispose();
		}

		private void ShowDefaultPresenter()
		{
			if (IsInCall && IsViewVisible)
				Navigation.NavigateTo<IActiveConferencePresenter>();
			else if (!IsInCall && IsViewVisible)
				Navigation.NavigateTo<IStartConferencePresenter>();
		}

		private void InitHeaderButtons()
		{
			var startConferenceButton = new HeaderButtonModel(0, 0, PresenterButtonPressed<IStartConferencePresenter>)
			{
				Icon = TouchCueIcons.GetIcon("videoconference", eTouchCueColor.White),
				LabelText = "Start/Join",
				Mode = eHeaderButtonMode.Orange
			};

			var activeConferenceButton = new HeaderButtonModel(0, 1, PresenterButtonPressed<IActiveConferencePresenter>)
			{
				Icon = TouchCueIcons.GetIcon("videoconference", eTouchCueColor.White),
				LabelText = "Participants",
				Mode = eHeaderButtonMode.Orange
			};
			var contactsButton = new HeaderButtonModel(0, 2, PresenterButtonPressed<IContactListPresenter>)
			{
				Icon = TouchCueIcons.GetIcon("audioconference", eTouchCueColor.White),
				LabelText = "Contacts",
				Mode = eHeaderButtonMode.Orange
			};
			var shareButton = new HeaderButtonModel(0, 3, PresenterButtonPressed<IShareConferencePresenter>)
			{
				Icon = TouchCueIcons.GetIcon("share_source", eTouchCueColor.White),
				LabelText = "Share",
				Mode = eHeaderButtonMode.Orange
			};

			m_EndConferenceButton = new HeaderButtonModel(1, 2, ConfirmEndConference)
			{
				LabelText = "End Call",
				Icon = TouchCueIcons.GetIcon("hangup", eTouchCueColor.White),
				Mode = eHeaderButtonMode.Red
			};
			var leaveConferenceButton = new HeaderButtonModel(1, 1, ConfirmLeaveConference)
			{
				LabelText = "Leave Call",
				Icon = TouchCueIcons.GetIcon("exit", eTouchCueColor.White),
				Mode = eHeaderButtonMode.Red
			};
			m_HideCameraButton = new HeaderButtonModel(1, 0, HideCameraCallback)
			{
				LabelText = "Show Camera",
				Icon = TouchCueIcons.GetIcon("reveal", eTouchCueColor.White),
				Mode = eHeaderButtonMode.Blue
			};
			
			m_PresenterButtons.Add(startConferenceButton, Navigation.LazyLoadPresenter<IStartConferencePresenter>());
			m_PresenterButtons.Add(activeConferenceButton, Navigation.LazyLoadPresenter<IActiveConferencePresenter>());
			m_PresenterButtons.Add(contactsButton, Navigation.LazyLoadPresenter<IContactListPresenter>());
			m_PresenterButtons.Add(shareButton, Navigation.LazyLoadPresenter<IShareConferencePresenter>());

			m_OutOfCallButtons.Add(startConferenceButton);
			m_OutOfCallButtons.Add(contactsButton);

			m_InCallButtons.Add(activeConferenceButton);
			m_InCallButtons.Add(contactsButton);
			m_InCallButtons.Add(shareButton);
			m_InCallButtons.Add(leaveConferenceButton);
			m_InCallButtons.Add(m_HideCameraButton);
		}

		protected override void Refresh(IConferenceBaseView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				foreach (var presenterButton in m_PresenterButtons)
				{
					HeaderButtonModel button = presenterButton.Key;
					ITouchDisplayPresenter presenter = presenterButton.Value;
					button.Selected = presenter.IsViewVisible;
				}

				var webConferenceControl = ActiveConferenceControl as IWebConferenceDeviceControl;
				bool cameraActive = webConferenceControl != null && webConferenceControl.CameraEnabled;
				m_HideCameraButton.LabelText = cameraActive ? "Hide Camera" : "Show Camera";
				m_HideCameraButton.Icon = TouchCueIcons.GetIcon(cameraActive ? "hide" : "reveal", eTouchCueColor.White);
				
				// Only hosts can end meeting for everyone
				ZoomRoom zoomRoom = webConferenceControl == null ? null : webConferenceControl.Parent as ZoomRoom;
				CallComponent component = zoomRoom == null ? null : zoomRoom.Components.GetComponent<CallComponent>();
				
				if (component != null && component.AmIHost && IsInCall && IsViewVisible)
					m_Header.AddRightButton(m_EndConferenceButton);
				else
					m_Header.RemoveRightButton(m_EndConferenceButton);
				m_Header.Refresh();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void SetControl(IDeviceControl control)
		{
			IConferenceDeviceControl conferenceControl = control as IConferenceDeviceControl;
			if (conferenceControl == null)
				return;

			ActiveConferenceControl = conferenceControl;
		}

		public bool SupportsControl(IDeviceControl control)
		{
			return control is IWebConferenceDeviceControl;
		}

		private void UpdateIsInCall()
		{
			IsInCall =
				m_SubscribedConferenceControl != null &&
				m_SubscribedConferenceControl.GetActiveConference() != null;
			UpdateButtonVisibility();
			Refresh();
		}

		private void UpdateButtonVisibility()
		{
			if (!IsViewVisible)
			{
				RemoveInCallButtons();
				RemoveOutOfCallButtons();
			}
			else if (IsInCall)
			{
				RemoveOutOfCallButtons();
				AddInCallButtons();
			}
			else
			{
				RemoveInCallButtons();
				AddOutOfCallButtons();
			}
			
			m_Header.Refresh();
		}

		private void AddInCallButtons()
		{
			foreach (var button in m_InCallButtons)
				m_Header.AddRightButton(button);
		}

		private void RemoveInCallButtons()
		{
			foreach (var button in m_InCallButtons)
				m_Header.RemoveRightButton(button);
		}

		private void AddOutOfCallButtons()
		{
			foreach (var button in m_OutOfCallButtons)
				m_Header.AddRightButton(button);
		}

		private void RemoveOutOfCallButtons()
		{
			foreach (var button in m_OutOfCallButtons)
				m_Header.RemoveRightButton(button);
		}

		#region Header Button Callbacks

		private void PresenterButtonPressed<TPresenter>() where TPresenter : class, ITouchDisplayPresenter
		{
			TPresenter presenter = Navigation.LazyLoadPresenter<TPresenter>();
			presenter.ShowView(!presenter.IsViewVisible);
			Refresh();
		}

		private void HideCameraCallback()
		{
			var webConferenceControl = ActiveConferenceControl as IWebConferenceDeviceControl;
			if (webConferenceControl == null)
				return;

			webConferenceControl.SetCameraEnabled(!webConferenceControl.CameraEnabled);
		}

		private void ConfirmLeaveConference()
		{
			Navigation.LazyLoadPresenter<IConfirmEndMeetingPresenter>()
				.Show("Are you sure you would like to leave the conference?", LeaveConferenceCallback);
		}

		private void ConfirmEndConference()
		{
			Navigation.LazyLoadPresenter<IConfirmEndMeetingPresenter>()
				.Show("Are you sure you would like to end the conference?", EndConferenceCallback);
		}

		private void LeaveConferenceCallback()
		{
			if (ActiveConferenceControl == null)
				return;

			var conference = ActiveConferenceControl.GetActiveConference() as IWebConference;
			if (conference == null)
				return;

			conference.LeaveConference();
		}

		private void EndConferenceCallback()
		{
			if (ActiveConferenceControl == null)
				return;

			var conference = ActiveConferenceControl.GetActiveConference() as IWebConference;
			if (conference == null)
				return;

			conference.EndConference();
		}

		#endregion

		#region Conference Control Callbacks

		private void Subscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);

			var webConferenceControl = control as IWebConferenceDeviceControl;
			if (webConferenceControl != null)
			{
				webConferenceControl.OnCameraEnabledChanged += WebConferenceControlOnOnCameraEnabledChanged;
				webConferenceControl.OnAmIHostChanged += WebConferenceControlOnOnAmIHostChanged;
			}

			UpdateIsInCall();
		}

		private void Unsubscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);

			var webConferenceControl = control as IWebConferenceDeviceControl;
			if (webConferenceControl != null)
			{
				webConferenceControl.OnCameraEnabledChanged -= WebConferenceControlOnOnCameraEnabledChanged;
				webConferenceControl.OnAmIHostChanged -= WebConferenceControlOnOnAmIHostChanged;
			}

			UpdateIsInCall();
		}

		private void WebConferenceControlOnOnAmIHostChanged(object sender, BoolEventArgs e)
		{
			Refresh();
		}

		private void ControlOnOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			UpdateIsInCall();
		}

		private void ControlOnOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			UpdateIsInCall();
		}
		
		private void WebConferenceControlOnOnCameraEnabledChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		/// <summary>
		/// Subscribe to the conference events.
		/// </summary>
		/// <param name="conference"></param>
		private void Subscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference events.
		/// </summary>
		/// <param name="conference"></param>
		private void Unsubscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		/// <summary>
		/// Called when a conference status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			switch (args.Data)
			{
				case eConferenceStatus.Connecting:
					Navigation.LazyLoadPresenter<IConferenceConnectingPresenter>().Show(CONNECTING_TEXT);
					break;
				case eConferenceStatus.Connected:
					Navigation.LazyLoadPresenter<IConferenceConnectingPresenter>().ShowView(false);
					// TODO temporary fix until we get layout control page
					ActiveConferenceControl.Parent.Controls.GetControl<ZoomRoomLayoutControl>().SetLayoutPosition(eZoomLayoutPosition.DownRight);
					break;
				case eConferenceStatus.Disconnecting:
					Navigation.LazyLoadPresenter<IConferenceConnectingPresenter>().Show(DISCONNECTING_TEXT);
					break;
				default:
					Navigation.LazyLoadPresenter<IConferenceConnectingPresenter>().ShowView(false);
					break;
			}

			UpdateIsInCall();
		}

		#endregion

		#region Conference Presenter Callbacks

		private void Subscribe(IConferencePresenter presenter)
		{
			presenter.OnViewVisibilityChanged += PresenterOnOnViewVisibilityChanged;
		}

		private void Unsubscribe(IConferencePresenter presenter)
		{
			presenter.OnViewVisibilityChanged -= PresenterOnOnViewVisibilityChanged;
		}

		private void PresenterOnOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			Refresh();
		}

		#endregion

		#region View Callbacks

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (!args.Data)
			{
				foreach (var presenter in m_ConferencePresenters)
					presenter.ShowView(false);

				if (Room != null)
					Room.FocusSource = null;
			}
			else
				ShowDefaultPresenter();

			UpdateButtonVisibility();

			Refresh();
		}

		#endregion
	}
}
