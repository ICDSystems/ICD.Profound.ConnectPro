using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Controls;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.AudioConference
{
	public sealed class AtcBasePresenter : AbstractPresenter<IAtcBaseView>, IAtcBasePresenter
	{
		private readonly KeypadStringBuilder m_Builder;
		private readonly SafeCriticalSection m_RefreshSection;

		private IDialingDeviceControl m_SubscribedAudioDialer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public AtcBasePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Builder = new KeypadStringBuilder();
			m_RefreshSection = new SafeCriticalSection();

			m_Builder.OnStringChanged += BuilderOnStringChanged;
		}

		/// <summary>
		/// Called when the dial string updates.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void BuilderOnStringChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IAtcBaseView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IConferenceSource active = GetActiveSource();
				eConferenceSourceStatus status = active == null ? eConferenceSourceStatus.Disconnected : active.Status;

				string activeStatus = StringUtils.NiceName(status);
				string dialString = m_Builder.ToString();
				bool inACall = active != null;

				// TODO
				view.SetRoomNumber("(484)713-9601");
				view.SetDialNumber(string.IsNullOrEmpty(dialString) ? "Dial Number" : dialString);
				view.SetCallStatus(activeStatus);

				view.SetClearButtonEnabled(dialString.Length > 0);
				view.SetDialButtonEnabled(dialString.Length > 0);
				view.SetHangupButtonEnabled(inACall);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Gets the audio dialer to monitor for incoming calls.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IDialingDeviceControl GetAudioDialer(IConnectProRoom room)
		{
			IConferenceManager manager = room == null ? null : room.ConferenceManager;
			return manager == null ? null : manager.GetDialingProvider(eConferenceSourceType.Audio);
		}

		/// <summary>
		/// Gets the active conference source.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IConferenceSource GetActiveSource()
		{
			return
				m_SubscribedAudioDialer == null
					? null
					: m_SubscribedAudioDialer.GetSources().FirstOrDefault(s => s.GetIsOnline());
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			m_SubscribedAudioDialer = GetAudioDialer(room);
			if (m_SubscribedAudioDialer == null)
				return;

			m_SubscribedAudioDialer.OnSourceAdded += AudioDialerOnSourceAdded;
			m_SubscribedAudioDialer.OnSourceRemoved += AudioDialerOnSourceRemoved;
			m_SubscribedAudioDialer.OnSourceChanged += AudioDialerOnSourceChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedAudioDialer == null)
				return;

			m_SubscribedAudioDialer.OnSourceAdded -= AudioDialerOnSourceAdded;
			m_SubscribedAudioDialer.OnSourceRemoved -= AudioDialerOnSourceRemoved;
			m_SubscribedAudioDialer.OnSourceChanged -= AudioDialerOnSourceChanged;

			m_SubscribedAudioDialer = null;
		}

		private void AudioDialerOnSourceAdded(object sender, ConferenceSourceEventArgs e)
		{
			RefreshIfVisible();
		}

		private void AudioDialerOnSourceRemoved(object sender, ConferenceSourceEventArgs e)
		{
			if (GetActiveSource() == null)
				m_Builder.Clear();

			RefreshIfVisible();
		}

		private void AudioDialerOnSourceChanged(object sender, ConferenceSourceEventArgs e)
		{
			if (GetActiveSource() == null)
				m_Builder.Clear();

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IAtcBaseView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnDialButtonPressed += ViewOnDialButtonPressed;
			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IAtcBaseView view)
		{
			base.Unsubscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnDialButtonPressed -= ViewOnDialButtonPressed;
			view.OnHangupButtonPressed -= ViewOnHangupButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
		}

		private void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		private void ViewOnKeypadButtonPressed(object sender, CharEventArgs eventArgs)
		{
			if (m_SubscribedAudioDialer == null)
				return;

			// DTMF
			foreach (IConferenceSource source in m_SubscribedAudioDialer.GetSources().Where(s => s.GetIsOnline()))
				source.SendDtmf(eventArgs.Data);

			m_Builder.AppendCharacter(eventArgs.Data);
		}

		private void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_SubscribedAudioDialer == null)
				return;

			m_SubscribedAudioDialer.GetSources().ForEach(s => s.Hangup());
		}

		private void ViewOnDialButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_SubscribedAudioDialer == null)
				return;

			string dialString = m_Builder.ToString();
			if (string.IsNullOrEmpty(dialString))
				return;

			m_SubscribedAudioDialer.Dial(dialString);
		}

		private void ViewOnClearButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Builder.Clear();
		}

		#endregion
	}
}
