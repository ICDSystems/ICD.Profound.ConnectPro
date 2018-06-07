using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.AudioConference
{
	public sealed class AtcIncomingCallPresenter : AbstractPresenter<IAtcIncomingCallView>, IAtcIncomingCallPresenter
	{
		/// <summary>
		/// Raised when the user answers the incoming call.
		/// </summary>
		public event EventHandler OnCallAnswered;

		private readonly List<IConferenceSource> m_Sources;
		private readonly SafeCriticalSection m_SourcesSection;
		private readonly SafeCriticalSection m_RefreshSection;

		[CanBeNull]
		private IDialingDeviceControl m_SubscribedAudioDialer;

		/// <summary>
		/// Gets the number of incoming sources.
		/// </summary>
		private int SourceCount { get { return m_SourcesSection.Execute(() => m_Sources.Count); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public AtcIncomingCallPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Sources = new List<IConferenceSource>();
			m_SourcesSection = new SafeCriticalSection();
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCallAnswered = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IAtcIncomingCallView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IConferenceSource source = GetFirstSource();
				string info = source == null ? string.Empty : string.Format("{0} - {1}", source.Name, source.Number);

				view.SetCallerInfo(info);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Gets the first unanswered source.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IConferenceSource GetFirstSource()
		{
			return m_SourcesSection.Execute(() => m_Sources.FirstOrDefault());
		}

		/// <summary>
		/// Gets the audio dialer to monitor for incoming calls.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IDialingDeviceControl GetAudioDialer(IConnectProRoom room)
		{
			IConferenceManager manager = room == null ? null : room.ConferenceManager;
			if (manager == null)
				return null;

			IDialingDeviceControl video = manager.GetDialingProvider(eConferenceSourceType.Video);
			IDialingDeviceControl audio = manager.GetDialingProvider(eConferenceSourceType.Audio);

			// Let the incoming video call popup take care of it
			if (video == audio)
				return null;

			return audio;
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

			m_SubscribedAudioDialer = null;
		}

		/// <summary>
		/// Adds the source to the collection.
		/// </summary>
		/// <param name="source"></param>
		private void AddSource(IConferenceSource source)
		{
			m_SourcesSection.Enter();

			try
			{
				if (m_Sources.Contains(source))
					return;

				m_Sources.Add(source);
				Subscribe(source);
			}
			finally
			{
				m_SourcesSection.Leave();
			}

			ShowView(SourceCount > 0);
			RefreshIfVisible(false);
		}

		/// <summary>
		/// Removes the source from the collection.
		/// </summary>
		/// <param name="source"></param>
		private void RemoveSource(IConferenceSource source)
		{
			m_SourcesSection.Enter();
			try
			{
				if (!m_Sources.Contains(source))
					return;

				m_Sources.Remove(source);
				Unsubscribe(source);
			}
			finally
			{
				m_SourcesSection.Leave();
			}

			RefreshIfVisible();
			ShowView(SourceCount > 0);
		}

		/// <summary>
		/// Called when a new source is detected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AudioDialerOnSourceAdded(object sender, ConferenceSourceEventArgs args)
		{
			IConferenceSource source = args.Data;
			if (source.GetIsRingingIncomingCall())
				AddSource(source);
		}

		#endregion

		#region Source Callbacks

		/// <summary>
		/// Subscribe to the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Subscribe(IConferenceSource source)
		{
			source.OnNameChanged += SourceOnNameChanged;
			source.OnNumberChanged += SourceOnNumberChanged;
			source.OnSourceTypeChanged += SourceOnSourceTypeChanged;
			source.OnStatusChanged += SourceOnStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Unsubscribe(IConferenceSource source)
		{
			source.OnNameChanged -= SourceOnNameChanged;
			source.OnNumberChanged -= SourceOnNumberChanged;
			source.OnSourceTypeChanged -= SourceOnSourceTypeChanged;
			source.OnStatusChanged -= SourceOnStatusChanged;
		}

		/// <summary>
		/// Called when the source status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnStatusChanged(object sender, ConferenceSourceStatusEventArgs args)
		{
			IConferenceSource source = sender as IConferenceSource;
			if (sender == null)
				return;

			if (!source.GetIsRingingIncomingCall())
				RemoveSource(source);
		}

		/// <summary>
		/// Called when the source type changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnSourceTypeChanged(object sender, EventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the source number changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnNumberChanged(object sender, StringEventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the source name changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnNameChanged(object sender, StringEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IAtcIncomingCallView view)
		{
			base.Subscribe(view);

			view.OnAnswerButtonPressed += ViewOnAnswerButtonPressed;
			view.OnIgnoreButtonPressed += ViewOnIgnoreButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IAtcIncomingCallView view)
		{
			base.Unsubscribe(view);

			view.OnAnswerButtonPressed -= ViewOnAnswerButtonPressed;
			view.OnIgnoreButtonPressed -= ViewOnIgnoreButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the ignore button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnIgnoreButtonPressed(object sender, EventArgs e)
		{
			IConferenceSource source = GetFirstSource();

			if (source != null)
				source.Hangup();

			ShowView(false);
		}

		/// <summary>
		/// Called when the user presses the answer button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnAnswerButtonPressed(object sender, EventArgs e)
		{
			IConferenceSource source = GetFirstSource();

			if (source != null)
				source.Answer();

			OnCallAnswered.Raise(this);

			ShowView(false);
		}

		#endregion
	}
}
