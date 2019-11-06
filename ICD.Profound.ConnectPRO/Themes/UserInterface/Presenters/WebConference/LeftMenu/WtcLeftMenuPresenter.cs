using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu
{
	[PresenterBinding(typeof(IWtcLeftMenuPresenter))]
	public sealed class WtcLeftMenuPresenter : AbstractWtcPresenter<IWtcLeftMenuView>, IWtcLeftMenuPresenter
	{
		private static readonly Type[] s_DefaultButtons =
		{
			typeof(IStartJoinMeetingWtcReferencedLeftMenuPresenter),
			typeof(IContactsWtcReferencedLeftMenuPresenter),
			typeof(ICallOutWtcReferencedLeftMenuPresenter)
		};

		private static readonly Type[] s_WebConferenceButtons =
		{
			typeof(IStartJoinMeetingWtcReferencedLeftMenuPresenter),
			typeof(IContactsWtcReferencedLeftMenuPresenter),
			typeof(IShareWtcReferencedLeftMenuPresenter),
			typeof(IRecordWtcReferencedLeftMenuPresenter)
		};

		private static readonly Type[] s_CallOutButtons =
		{
			typeof(IStartJoinMeetingWtcReferencedLeftMenuPresenter),
			typeof(IContactsWtcReferencedLeftMenuPresenter),
			typeof(ICallOutWtcReferencedLeftMenuPresenter)
		};

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly WtcReferencedLeftMenuPresenterFactory m_ChildFactory;

		private Type[] m_Buttons;
		private eMode? m_Mode;
		private bool m_IsInWebConference;

		public eMode Mode
		{
			get { return m_Mode ?? eMode.Default; }
			set
			{
				if (value == m_Mode)
					return;

				m_Mode = value;
				m_Buttons = GetButtonPresenterTypes().ToArray();

				if (IsViewVisible)
					ShowDefaultPresenterForMode();

				RefreshIfVisible();
			}
		}

		private bool IsInWebConference
		{
			get { return m_IsInWebConference; }
			set
			{
				if (value == m_IsInWebConference)
					return;

				m_IsInWebConference = value;

				// Flip to the new mode when entering/leaving a web conference
				if (Mode == eMode.WebConference || Mode == eMode.Default)
					Mode = m_IsInWebConference
						? eMode.WebConference
						: eMode.Default;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildFactory = new WtcReferencedLeftMenuPresenterFactory(nav, ChildItemFactory, p => { }, p => { });

			Mode = eMode.Default;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_ChildFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcLeftMenuView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				foreach (IWtcReferencedLeftMenuPresenter presenter in m_ChildFactory.BuildChildren(m_Buttons))
				{
					presenter.ActiveConferenceControl = ActiveConferenceControl;
					presenter.ShowView(true);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		/// <summary>
		/// Not in Call = Start/Join + Contacts + Call Out
		/// In Video Call = Active Meeting + Contacts + Share + Record
		/// Audio Call = Start/Join (Disabled) + Contacts (Disabled) + Call Out
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Type> GetButtonPresenterTypes()
		{
			switch (Mode)
			{
				case eMode.Default:
					return s_DefaultButtons;
				case eMode.WebConference:
					return s_WebConferenceButtons;
				case eMode.CallOut:
					return s_CallOutButtons;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Gets the views for the child buttons.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IWtcReferencedLeftMenuView> ChildItemFactory(ushort count)
		{
			return GetView().GetChildViews(ViewFactory, count);
		}

		private void UpdateIsInWebConference()
		{
			IsInWebConference =
				ActiveConferenceControl != null &&
				ActiveConferenceControl.GetActiveConference() != null;
		}

		private void ShowDefaultPresenterForMode()
		{
			switch (m_Mode)
			{
				case eMode.Default:
					Navigation.NavigateTo<IWtcStartMeetingPresenter>();
					break;

				case eMode.WebConference:
					Navigation.NavigateTo<IWtcActiveMeetingPresenter>();
					break;

				case eMode.CallOut:
					Navigation.NavigateTo<IWtcCallOutPresenter>();
					break;
			}
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

			UpdateIsInWebConference();
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
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data as IWebConference);
			UpdateIsInWebConference();
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data as IWebConference);
			UpdateIsInWebConference();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IWebConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnParticipantRemoved;
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IWebConference conference)
		{
			conference.OnParticipantAdded -= ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved -= ConferenceOnParticipantRemoved;
			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			UpdateIsInWebConference();
		}

		private void ConferenceOnParticipantAdded(object sender, ParticipantEventArgs participantEventArgs)
		{
			UpdateIsInWebConference();
		}

		private void ConferenceOnParticipantRemoved(object sender, ParticipantEventArgs participantEventArgs)
		{
			UpdateIsInWebConference();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (args.Data)
				ShowDefaultPresenterForMode();
		}

		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			// If the Parent will be hidden, hide all the children first.
			if (!args.Data)
			{
				foreach (IWtcReferencedLeftMenuPresenter child in m_ChildFactory)
					child.HideSubpages();
			}
		}

		#endregion
	}
}