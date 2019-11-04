using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu
{
	[PresenterBinding(typeof(IWtcLeftMenuPresenter))]
	public sealed class WtcLeftMenuPresenter : AbstractWtcPresenter<IWtcLeftMenuView>, IWtcLeftMenuPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly WtcReferencedLeftMenuPresenterFactory m_ChildFactory;

		private IWtcLeftMenuButtonModel[] m_Buttons;

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
				m_Buttons = BuildButtons().ToArray();
				foreach (IWtcReferencedLeftMenuPresenter presenter in m_ChildFactory.BuildChildren(m_Buttons))
				{
					presenter.ShowView(true);
					presenter.SetViewEnabled(presenter.ButtonModel.Enabled);
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
		private IEnumerable<IWtcLeftMenuButtonModel> BuildButtons()
		{
			throw new System.NotImplementedException();
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
		}

		/// <summary>
		/// Unsusbcribe from the conference control events.
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

			RefreshIfVisible();
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data as IWebConference);

			RefreshIfVisible();
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
	}
}