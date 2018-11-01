﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
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
		public event EventHandler<GenericEventArgs<IConferenceDeviceControl>>  OnCallAnswered;

		private readonly Dictionary<IIncomingCall, IConferenceDeviceControl> m_IncomingCalls;
		private readonly SafeCriticalSection m_IncomingCallsSection;
		private readonly SafeCriticalSection m_RefreshSection;

		[CanBeNull]
		private List<IConferenceDeviceControl> m_SubscribedAudioDialers;

		/// <summary>
		/// Gets the number of incoming sources.
		/// </summary>
		private int SourceCount { get { return m_IncomingCallsSection.Execute(() => m_IncomingCalls.Count); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public AtcIncomingCallPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_IncomingCalls = new Dictionary<IIncomingCall, IConferenceDeviceControl>();
			m_IncomingCallsSection = new SafeCriticalSection();
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
				IIncomingCall source = GetFirstSource();
				string info = GetCallerInfo(source);

				view.SetCallerInfo(info);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private static string GetCallerInfo(IIncomingCall call)
		{
			string output = string.Empty;

			if (call == null)
				return output;

			output = string.IsNullOrEmpty(call.Name) ? "Unknown" : call.Name;

			if (!string.IsNullOrEmpty(call.Number))
				output = string.Format("{0} - {1}", output, call.Number);

			return output;
		}

		/// <summary>
		/// Gets the first unanswered call.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IIncomingCall GetFirstSource()
		{
			return m_IncomingCallsSection.Execute(() => m_IncomingCalls.Select(p => p.Key).FirstOrDefault());
		}

		/// <summary>
		/// Gets the audio dialer to monitor for incoming calls.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IEnumerable<IConferenceDeviceControl> GetAudioDialers(IConnectProRoom room)
		{
			return room.Originators.GetInstancesRecursive<ISource>()
				.Select(s => room.Core.Originators[s.Device] as IDevice)
				.SelectMany(d => d == null ? Enumerable.Empty<IConferenceDeviceControl>() : d.Controls.GetControls<IConferenceDeviceControl>())
				.Where(c => c.Supports == eCallType.Audio);
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			m_SubscribedAudioDialers = GetAudioDialers(room).ToList();
			foreach (var dialer in m_SubscribedAudioDialers)
			{
				dialer.OnIncomingCallAdded += AudioDialerOnIncomingCallAdded;
				dialer.OnIncomingCallRemoved += AudioDialerOnIncomingCallRemoved;
			}
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedAudioDialers == null)
				return;

			foreach (var dialer in m_SubscribedAudioDialers)
			{
				dialer.OnIncomingCallAdded -= AudioDialerOnIncomingCallAdded;
				dialer.OnIncomingCallRemoved -= AudioDialerOnIncomingCallRemoved;
			}
		}

		/// <summary>
		/// Adds the call to the collection.
		/// </summary>
		/// <param name="call"></param>
		/// <param name="control"></param>
		private void AddIncomingCall(IIncomingCall call, IConferenceDeviceControl control)
		{
			m_IncomingCallsSection.Enter();

			try
			{
				if (m_IncomingCalls.ContainsKey(call))
					return;

				m_IncomingCalls.Add(call, control);
				Subscribe(call);
			}
			finally
			{
				m_IncomingCallsSection.Leave();
			}

			ShowView(SourceCount > 0);
			RefreshIfVisible(false);
		}

		/// <summary>
		/// Removes the call from the collection.
		/// </summary>
		/// <param name="call"></param>
		private void RemoveIncomingCall(IIncomingCall call)
		{
			m_IncomingCallsSection.Enter();
			try
			{
				if (!m_IncomingCalls.ContainsKey(call))
					return;

				m_IncomingCalls.Remove(call);
				Unsubscribe(call);
			}
			finally
			{
				m_IncomingCallsSection.Leave();
			}

			RefreshIfVisible();
			ShowView(SourceCount > 0);
		}

		/// <summary>
		/// Called when a new call is detected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AudioDialerOnIncomingCallAdded(object sender, GenericEventArgs<IIncomingCall> args)
		{
			IIncomingCall call = args.Data;
			if (call.GetIsRingingIncomingCall())
				AddIncomingCall(call, sender as IConferenceDeviceControl);
		}

		/// <summary>
		/// Called when a new call is detected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AudioDialerOnIncomingCallRemoved(object sender, GenericEventArgs<IIncomingCall> args)
		{
			IIncomingCall call = args.Data;
			RemoveIncomingCall(call);
		}

		#endregion

		#region Source Callbacks

		/// <summary>
		/// Subscribe to the call events.
		/// </summary>
		/// <param name="call"></param>
		private void Subscribe(IIncomingCall call)
		{
			call.OnAnswerStateChanged += CallOnAnswerStateChanged;
			call.OnNameChanged += CallOnNameChanged;
			call.OnNumberChanged += CallOnNumberChanged;
		}

		/// <summary>
		/// Unsubscribe from the call events.
		/// </summary>
		/// <param name="call"></param>
		private void Unsubscribe(IIncomingCall call)
		{
			call.OnAnswerStateChanged -= CallOnAnswerStateChanged;
			call.OnNameChanged -= CallOnNameChanged;
			call.OnNumberChanged -= CallOnNumberChanged;
		}

		/// <summary>
		/// Called when the call status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CallOnAnswerStateChanged(object sender, IncomingCallAnswerStateEventArgs args)
		{
			var source = sender as IIncomingCall;
			if (sender == null)
				return;

			if (!source.GetIsRingingIncomingCall())
				RemoveIncomingCall(source);
		}

		/// <summary>
		/// Called when the call number changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CallOnNumberChanged(object sender, StringEventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the call name changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CallOnNameChanged(object sender, StringEventArgs args)
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
			IIncomingCall call = GetFirstSource();

			if (call != null)
				call.Reject();

			ShowView(false);
		}

		/// <summary>
		/// Called when the user presses the answer button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnAnswerButtonPressed(object sender, EventArgs e)
		{
			IIncomingCall call = GetFirstSource();
			IConferenceDeviceControl control = null;

			m_IncomingCallsSection.Enter();
			try
			{
				if (m_IncomingCalls.ContainsKey(call))
					control = m_IncomingCalls[call];
			}
			finally
			{
				m_IncomingCallsSection.Leave();
			}

			if (call != null)
				call.Answer();

			if (Room != null)
				Room.StartMeeting(false);
			OnCallAnswered.Raise(this, new GenericEventArgs<IConferenceDeviceControl>(control));

			ShowView(false);
		}

		#endregion
	}
}
