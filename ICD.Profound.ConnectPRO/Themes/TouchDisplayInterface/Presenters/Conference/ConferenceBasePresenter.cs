using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.Devices.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IConferenceBasePresenter))]
	public sealed class ConferenceBasePresenter : AbstractPopupPresenter<IConferenceBaseView>, IConferenceBasePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly List<IConferencePresenter> m_ConferencePresenters;
		private readonly Dictionary<HeaderButtonModel, ITouchDisplayPresenter> m_PresenterButtons;
		private readonly List<HeaderButtonModel> m_InCallButtons;
		private readonly List<HeaderButtonModel> m_OutOfCallButtons;

		private HeaderButtonModel m_HideCameraButton;
		private HeaderButtonModel m_EndConferenceButton;
		
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
			m_RefreshSection = new SafeCriticalSection();
			m_ConferencePresenters = Navigation.LazyLoadPresenters<IConferencePresenter>().ToList();
			foreach (var presenter in m_ConferencePresenters)
				Subscribe(presenter);

			m_PresenterButtons = new Dictionary<HeaderButtonModel, ITouchDisplayPresenter>();
			m_InCallButtons = new List<HeaderButtonModel>();
			m_OutOfCallButtons = new List<HeaderButtonModel>();
			InitHeaderButtons();
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
				Icon = TouchCueIcons.GetIcon("videoconference"),
				LabelText = "Start/Join",
				Mode = eHeaderButtonMode.Orange
			};

			var activeConferenceButton = new HeaderButtonModel(0, 1, PresenterButtonPressed<IActiveConferencePresenter>)
			{
				Icon = TouchCueIcons.GetIcon("videoconference"),
				LabelText = "Participants",
				Mode = eHeaderButtonMode.Orange
			};
			var contactsButton = new HeaderButtonModel(0, 2, PresenterButtonPressed<IContactListPresenter>)
			{
				Icon = TouchCueIcons.GetIcon("audiocall"),
				LabelText = "Contacts",
				Mode = eHeaderButtonMode.Orange
			};
			var shareButton = new HeaderButtonModel(0, 3, PresenterButtonPressed<IShareConferencePresenter>)
			{
				Icon = TouchCueIcons.GetIcon("share_source"),
				LabelText = "Share",
				Mode = eHeaderButtonMode.Orange
			};

			m_EndConferenceButton = new HeaderButtonModel(1, 2, EndConferenceCallback)
			{
				LabelText = "End Call",
				Icon = TouchCueIcons.GetIcon("hangup"),
				Mode = eHeaderButtonMode.Red
			};
			var leaveConferenceButton = new HeaderButtonModel(1, 1, LeaveConferenceCallback)
			{
				LabelText = "Leave Call",
				Icon = TouchCueIcons.GetIcon("exit"),
				Mode = eHeaderButtonMode.Red
			};
			m_HideCameraButton = new HeaderButtonModel(1, 0, HideCameraCallback)
			{
				LabelText = "Show Camera",
				Icon = TouchCueIcons.GetIcon("reveal"),
				Mode = eHeaderButtonMode.Blue
			};
			
			m_PresenterButtons.Add(startConferenceButton, Navigation.LazyLoadPresenter<IStartConferencePresenter>());
			m_PresenterButtons.Add(activeConferenceButton, Navigation.LazyLoadPresenter<IActiveConferencePresenter>());
			m_PresenterButtons.Add(contactsButton, Navigation.LazyLoadPresenter<IContactListPresenter>());
			m_PresenterButtons.Add(shareButton, Navigation.LazyLoadPresenter<IShareConferencePresenter>());

			m_OutOfCallButtons.Add(startConferenceButton);

			m_InCallButtons.Add(activeConferenceButton);
			m_InCallButtons.Add(contactsButton);
			m_InCallButtons.Add(shareButton);
			m_InCallButtons.Add(m_EndConferenceButton);
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
				m_HideCameraButton.Icon = TouchCueIcons.GetIcon(cameraActive ? "hide" : "reveal");
				

				// Only hosts can end meeting for everyone
				ZoomRoom zoomRoom = webConferenceControl == null ? null : webConferenceControl.Parent as ZoomRoom;
				CallComponent component = zoomRoom == null ? null : zoomRoom.Components.GetComponent<CallComponent>();
				m_EndConferenceButton.Enabled = component != null && component.AmIHost;
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
				AddInCallButtons();
				RemoveOutOfCallButtons();
			}
			else
			{
				AddOutOfCallButtons();
				RemoveInCallButtons();
			}
		}

		private void AddInCallButtons()
		{
			var header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			foreach (var button in m_InCallButtons)
				header.AddRightButton(button);
		}

		private void RemoveInCallButtons()
		{
			var header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			foreach (var button in m_InCallButtons)
				header.RemoveRightButton(button);
		}

		private void AddOutOfCallButtons()
		{
			var header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			foreach (var button in m_OutOfCallButtons)
				header.AddRightButton(button);
		}

		private void RemoveOutOfCallButtons()
		{
			var header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			foreach (var button in m_OutOfCallButtons)
				header.RemoveRightButton(button);
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
				webConferenceControl.OnCameraEnabledChanged += WebConferenceControlOnOnCameraEnabledChanged;

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
				webConferenceControl.OnCameraEnabledChanged -= WebConferenceControlOnOnCameraEnabledChanged;

			UpdateIsInCall();
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
			//IGenericLoadingSpinnerPresenter spinner = Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>();

			//switch (args.Data)
			//{
			//	case eConferenceStatus.Connecting:
			//		spinner.ShowView("Connecting...", 30 * 1000);
			//		break;
			//	case eConferenceStatus.Connected:
			//		m_ConnectingTimer.Reset(1000); // hide connecting page 1 second after connection complete
			//		Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>().ShowView(false);
			//		break;
			//	default:
			//		spinner.ShowView(false);
			//		break;
			//}

			UpdateIsInCall();
		}

		#endregion

		#region Conference Presenter Callbacks

		private void Subscribe(IConferencePresenter presenter)
		{
			presenter.OnViewVisibilityChanged += PresenterOnOnViewVisibilityChanged;
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
