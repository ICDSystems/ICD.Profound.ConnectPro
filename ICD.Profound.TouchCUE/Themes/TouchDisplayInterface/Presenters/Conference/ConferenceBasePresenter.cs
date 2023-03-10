using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Layout;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Call;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Controls;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Responses;
using ICD.Connect.Devices.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.Camera;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Notifications;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IConferenceBasePresenter))]
	public sealed class ConferenceBasePresenter : AbstractPopupPresenter<IConferenceBaseView>, IConferenceBasePresenter
	{
		private const string DISCONNECTING_TEXT = "Disconnecting... Audio and video may still be live.";
		private const string CONNECTING_TEXT = "Please wait. Your conference is being connected...";

		#region Readonly Properties

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly List<IConferencePresenter> m_ConferencePresenters;
		private readonly Dictionary<HeaderButtonModel, ITouchDisplayPresenter> m_PresenterButtons;
		private readonly List<HeaderButtonModel> m_InCallButtons;
		private readonly List<HeaderButtonModel> m_OutOfCallButtons;

		#endregion

		#region Properties

		private HeaderButtonModel m_HideCameraButton;
		private HeaderButtonModel m_EndConferenceButton;
		private readonly IHeaderPresenter m_Header;
		
		private IConferenceDeviceControl m_SubscribedConferenceControl;
		private bool m_IsInCall;
		private readonly ICameraLayoutPresenter m_CameraLayoutPresenter;

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
				
				m_CameraLayoutPresenter.SetConferenceLayoutControl(value == null
					? null
					: value.Parent.Controls.GetControl<IConferenceLayoutControl>());

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

		#endregion

		public ConferenceBasePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_Header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			m_RefreshSection = new SafeCriticalSection();
			m_ConferencePresenters = Navigation.LazyLoadPresenters<IConferencePresenter>().ToList();
			foreach (var presenter in m_ConferencePresenters)
				Subscribe(presenter);

			m_CameraLayoutPresenter = Navigation.LazyLoadPresenter<ICameraLayoutPresenter>();

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

		protected override void Refresh(IConferenceBaseView view)
		{
			m_RefreshSection.Enter();
			try
			{
				foreach (var presenterButton in m_PresenterButtons)
				{
					HeaderButtonModel button = presenterButton.Key;
					ITouchDisplayPresenter presenter = presenterButton.Value;
					button.Selected = presenter.IsViewVisible;
				}

				bool cameraActive = ActiveConferenceControl != null && !ActiveConferenceControl.CameraMute;
				m_HideCameraButton.LabelText = cameraActive ? "Hide Camera" : "Show Camera";
				m_HideCameraButton.Icon = TouchCueIcons.GetIcon(cameraActive ? eTouchCueIcon.Hide : eTouchCueIcon.Reveal, eTouchCueColor.White);
				
				// Only hosts can end meeting for everyone
				ZoomRoom zoomRoom = ActiveConferenceControl == null ? null : ActiveConferenceControl.Parent as ZoomRoom;
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

		#region Private Methods

		private void ShowDefaultPresenter()
		{
			if (IsInCall && IsViewVisible)
				Navigation.NavigateTo<IActiveConferencePresenter>();
			else if (!IsInCall && IsViewVisible)
			{
				// hide record if leaving call
				Navigation.LazyLoadPresenter<IRecordConferencePresenter>().ShowView(false);

				Navigation.NavigateTo<IStartConferencePresenter>();
			}
				
		}

		private void UpdateIsInCall()
		{
			IsInCall =
				m_SubscribedConferenceControl != null &&
				m_SubscribedConferenceControl.GetActiveConferences().Any();
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

		private void InitHeaderButtons()
		{
			var startConferenceButton = new HeaderButtonModel(0, 0, PresenterButtonPressed<IStartConferencePresenter>)
			{
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.VideoConference, eTouchCueColor.White),
				LabelText = "Start/Join",
				Mode = eHeaderButtonMode.Orange
			};
			var callOutButton = new HeaderButtonModel(0, 3, PresenterButtonPressed<ICallOutConferencePresenter>)
			{
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.CallOut, eTouchCueColor.White),
				LabelText = "Call Out",
				Mode = eHeaderButtonMode.Orange
			};

			var activeConferenceButton = new HeaderButtonModel(0, 1, PresenterButtonPressed<IActiveConferencePresenter>)
			{
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.VideoConference, eTouchCueColor.White),
				LabelText = "Participants",
				Mode = eHeaderButtonMode.Orange
			};
			var contactsButton = new HeaderButtonModel(0, 2, PresenterButtonPressed<IContactListPresenter>)
			{
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.AudioConference, eTouchCueColor.White),
				LabelText = "Contacts",
				Mode = eHeaderButtonMode.Orange
			};
			var shareButton = new HeaderButtonModel(0, 3, PresenterButtonPressed<IShareConferencePresenter>)
			{
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.Share, eTouchCueColor.White),
				LabelText = "Share",
				Mode = eHeaderButtonMode.Orange
			};
			var layoutButton = new HeaderButtonModel(0, 4, PresenterButtonPressed<ICameraLayoutPresenter>)
			{
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.ConferenceCamera, eTouchCueColor.White),
				LabelText = "Layout",
				Mode = eHeaderButtonMode.Orange
			};
			var recordButton = new HeaderButtonModel(0, 5, PresenterButtonPressed<IRecordConferencePresenter>)
			{
				LabelText = "Record",
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.Record, eTouchCueColor.White),
				Mode = eHeaderButtonMode.Orange
			};

			m_EndConferenceButton = new HeaderButtonModel(1, 2, ConfirmEndConference)
			{
				LabelText = "End Call",
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.HangUp, eTouchCueColor.White),
				Mode = eHeaderButtonMode.Red
			};
			var leaveConferenceButton = new HeaderButtonModel(1, 1, ConfirmLeaveConference)
			{
				LabelText = "Leave Call",
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.Exit, eTouchCueColor.White),
				Mode = eHeaderButtonMode.Red
			};
			m_HideCameraButton = new HeaderButtonModel(1, 0, HideCameraCallback)
			{
				LabelText = "Show Camera",
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.Reveal, eTouchCueColor.White),
				Mode = eHeaderButtonMode.Blue
			};
			
			m_PresenterButtons.Add(startConferenceButton, Navigation.LazyLoadPresenter<IStartConferencePresenter>());
			m_PresenterButtons.Add(activeConferenceButton, Navigation.LazyLoadPresenter<IActiveConferencePresenter>());
			m_PresenterButtons.Add(contactsButton, Navigation.LazyLoadPresenter<IContactListPresenter>());
			m_PresenterButtons.Add(shareButton, Navigation.LazyLoadPresenter<IShareConferencePresenter>());
			m_PresenterButtons.Add(layoutButton, Navigation.LazyLoadPresenter<ICameraLayoutPresenter>());
			m_PresenterButtons.Add(recordButton, Navigation.LazyLoadPresenter<IRecordConferencePresenter>());
			m_PresenterButtons.Add(callOutButton, Navigation.LazyLoadPresenter<ICallOutConferencePresenter>());

			m_OutOfCallButtons.Add(startConferenceButton);
			m_OutOfCallButtons.Add(contactsButton);
			m_OutOfCallButtons.Add(callOutButton);

			m_InCallButtons.Add(activeConferenceButton);
			m_InCallButtons.Add(contactsButton);
			m_InCallButtons.Add(shareButton);
			m_InCallButtons.Add(layoutButton);
			m_InCallButtons.Add(recordButton);
			m_InCallButtons.Add(m_HideCameraButton);
			m_InCallButtons.Add(leaveConferenceButton);
		}

		#endregion

		#region Protected Methods

		public void SetControl(IDeviceControl control)
		{
			IConferenceDeviceControl conferenceControl = control as IConferenceDeviceControl;
			if (conferenceControl == null)
				return;

			ActiveConferenceControl = conferenceControl;
		}

		public bool SupportsControl(IDeviceControl control)
		{
			return control is IConferenceDeviceControl;
		}

		#endregion

		#region Header Button Callbacks

		private void PresenterButtonPressed<TPresenter>() where TPresenter : class, ITouchDisplayPresenter
		{
			TPresenter presenter = Navigation.LazyLoadPresenter<TPresenter>();
			presenter.ShowView(!presenter.IsViewVisible);
			Refresh();
		}

		private void HideCameraCallback()
		{
			if (ActiveConferenceControl == null || !ActiveConferenceControl.SupportedConferenceControlFeatures.HasFlag(eConferenceControlFeatures.CameraMute))
				return;

			ActiveConferenceControl.SetCameraMute(!ActiveConferenceControl.CameraMute);
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

			IEnumerable<IConference> conferences = ActiveConferenceControl.GetActiveConferences().ToArray();
			foreach (IConference conference in conferences)
				conference.LeaveConference();
		}

		private void EndConferenceCallback()
		{
			if (ActiveConferenceControl == null)
				return;

			IEnumerable<IConference> conferences = ActiveConferenceControl.GetActiveConferences().ToArray();
			foreach (IConference conference in conferences)
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
			control.OnCameraMuteChanged += WebConferenceControlOnCameraMuteChanged;
			control.OnAmIHostChanged += WebConferenceControlOnOnAmIHostChanged;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);

			UpdateIsInCall();
		}

		private void Unsubscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnOnConferenceRemoved;
			control.OnCameraMuteChanged -= WebConferenceControlOnCameraMuteChanged;
			control.OnAmIHostChanged -= WebConferenceControlOnOnAmIHostChanged;

			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);

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
		
		private void WebConferenceControlOnCameraMuteChanged(object sender, BoolEventArgs e)
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
					ActiveConferenceControl.Parent.Controls.GetControl<ZoomRoomLayoutControl>().SetLayoutPosition(eZoomLayoutPosition.DownRight);
					Navigation.LazyLoadPresenter<IConferenceConnectingPresenter>().ShowView(false);
					Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>().ShowView(false);
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
				Navigation.LazyLoadPresenter<ICameraLayoutPresenter>().ShowView(false);
				Navigation.LazyLoadPresenter<ICameraControlPresenter>().ShowView(false);

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
