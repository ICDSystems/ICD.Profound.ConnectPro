using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public class WtcLeftMenuPresenter : AbstractWtcPresenter<IWtcLeftMenuView>, IWtcLeftMenuPresenter
	{
		private const ushort INDEX_MEETING = 0;
		public const ushort INDEX_CONTACTS = 1;
		//private const ushort INDEX_CALL_OUT = 3;
		private const ushort INDEX_SHARE = 2;
		//private const ushort INDEX_RECORDING = 5;

		private static readonly Dictionary<ushort, string> s_ButtonLabels = new Dictionary<ushort, string>
		{
			{INDEX_MEETING, "Start/Join Meeting"},
			{INDEX_CONTACTS, "Contacts"},
			//{INDEX_CALL_OUT, "Call Out"},
			{INDEX_SHARE, "Share"},
			//{INDEX_RECORDING, "Record"},
		};

		private static readonly Dictionary<ushort, string> s_ButtonIcons = new Dictionary<ushort, string>
		{
			{INDEX_MEETING, "icon_videoConference_white"},
			{INDEX_CONTACTS, "icon_list_white"},
			//{INDEX_CALL_OUT, "icon_call_white"},
			{INDEX_SHARE, "icon_exit_white"},
			//{INDEX_RECORDING, "icon_tcRecord_white"},
		};

		private static readonly Dictionary<ushort, Type> s_NavTypes = new Dictionary<ushort, Type>
		{
			{INDEX_MEETING, typeof (IWtcStartMeetingPresenter)},
			{INDEX_CONTACTS, typeof(IWtcContactListPresenter)},
			//{INDEX_CALL_OUT, typeof(IWtcCallOutPresenter)},
			{INDEX_SHARE,  typeof (IWtcSharePresenter)},
			//{INDEX_RECORDING, typeof(IWtcRecordingPresenter)},
		};

		private readonly Dictionary<ushort, IPresenter> m_NavPages;
		private readonly SafeCriticalSection m_RefreshSection;

		private IPresenter m_Visible;

		public IPresenter Visible
		{
			get { return m_Visible; }
			set
			{
				if (value == m_Visible)
					return;

				m_Visible = value;

				RefreshIfVisible();
			}
		}

		public WtcLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_NavPages = new Dictionary<ushort, IPresenter>();
			foreach (KeyValuePair<ushort, Type> kvp in s_NavTypes)
				m_NavPages.Add(kvp.Key, nav.LazyLoadPresenter(kvp.Value));

			SubscribePages();
		}

		/// <inheritdoc />
		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			UnsubscribePages();
		}

		protected override void Refresh(IWtcLeftMenuView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				var activeMeetingPresenter = Navigation.LazyLoadPresenter<IWtcActiveMeetingPresenter>(); // for setting enabled state

				view.SetActiveMeetingIndicatorMode(IsInMeeting);

				foreach (KeyValuePair<ushort, IPresenter> kvp in m_NavPages)
				{
					view.SetButtonIcon(kvp.Key, s_ButtonIcons[kvp.Key]);
					view.SetButtonLabel(kvp.Key, kvp.Key == INDEX_MEETING && IsInMeeting 
						                             ? "Active Meeting" 
						                             : s_ButtonLabels[kvp.Key]);
					view.SetButtonVisible(kvp.Key, true);
					view.SetButtonEnabled(kvp.Key, kvp.Key != INDEX_SHARE || IsInMeeting); // share disabled if not in meeting
					view.SetButtonSelected(kvp.Key, kvp.Key == INDEX_MEETING && IsInMeeting
						                                ? activeMeetingPresenter.IsViewVisible
						                                : kvp.Value.IsViewVisible);
				}

				view.SetButtonCount((ushort)s_ButtonLabels.Count);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private bool m_IsInMeeting;
		private bool IsInMeeting
		{
			get { return m_IsInMeeting; }
			set
			{
				if (m_IsInMeeting == value)
					return;

				m_IsInMeeting = value;

				ShowMenu(INDEX_MEETING);

				RefreshIfVisible();
			}
		}

		public void ShowMenu(ushort index)
		{
			if (index == INDEX_MEETING && IsInMeeting)
				Navigation.LazyLoadPresenter<IWtcActiveMeetingPresenter>().ShowView(IsViewVisible);
			else
				m_NavPages[index].ShowView(IsViewVisible);
		}

		private void UpdateIsInMeeting()
		{
			IsInMeeting = ActiveConferenceControl != null && ActiveConferenceControl.GetActiveConference() != null;
		}

		#region Conference Control Callbacks

		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control != null)
			{
				control.OnConferenceAdded += ControlOnOnConferenceAdded;
				control.OnConferenceRemoved += ControlOnOnConferenceRemoved;

				foreach (var conference in control.GetConferences())
					Subscribe(conference);
			}

			UpdateIsInMeeting();
		}

		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control != null)
			{
				control.OnConferenceAdded -= ControlOnOnConferenceAdded;
				control.OnConferenceRemoved -= ControlOnOnConferenceRemoved;

				foreach (var conference in control.GetConferences())
					Unsubscribe(conference);
			}

			UpdateIsInMeeting();
		}

		private void ControlOnOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data);
			UpdateIsInMeeting();
		}

		private void ControlOnOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data);
			UpdateIsInMeeting();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			conference.OnStatusChanged += ConferenceOnOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			conference.OnStatusChanged -= ConferenceOnOnStatusChanged;
		}

		private void ConferenceOnOnStatusChanged(object sender, ConferenceStatusEventArgs e)
		{
			UpdateIsInMeeting();
		}

		#endregion

		#region Page Callbacks

		/// <summary>
		/// Subscribe to the child page events.
		/// </summary>
		private void SubscribePages()
		{
			foreach (IPresenter presenter in m_NavPages.Values)
				presenter.OnViewVisibilityChanged += PresenterOnViewVisibilityChanged;
			Navigation.LazyLoadPresenter<IWtcActiveMeetingPresenter>().OnViewVisibilityChanged += PresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the child page events.
		/// </summary>
		private void UnsubscribePages()
		{
			foreach (IPresenter presenter in m_NavPages.Values)
				presenter.OnViewVisibilityChanged -= PresenterOnViewVisibilityChanged;
			Navigation.LazyLoadPresenter<IWtcActiveMeetingPresenter>().OnViewVisibilityChanged -= PresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when a child page visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void PresenterOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			if (boolEventArgs.Data)
				Visible = sender as IPresenter;
			else if (Visible == sender)
				Visible = null;

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IWtcLeftMenuView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWtcLeftMenuView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		private void ViewOnButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			ShowMenu(eventArgs.Data);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);
			
			if (args.Data)
			{
				ShowMenu(INDEX_MEETING);
			}
			else
			{
				foreach (IPresenter presenter in m_NavPages.Values)
					presenter.ShowView(false);
			}
		}

		#endregion
	}
}