using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	[PresenterBinding(typeof(IVtcIncomingCallPresenter))]
	public sealed class VtcIncomingCallPresenter : AbstractUiPresenter<IVtcIncomingCallView>, IVtcIncomingCallPresenter
	{
		private readonly Dictionary<IIncomingCall, IConferenceDeviceControl> m_IncomingCalls;
		private readonly List<IConferenceDeviceControl> m_SubscribedConferenceControls;
		private readonly SafeCriticalSection m_IncomingCallsSection;
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Gets the number of incoming sources.
		/// </summary>
		private int IncomingCallCount { get { return m_IncomingCallsSection.Execute(() => m_IncomingCalls.Count); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcIncomingCallPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_IncomingCalls = new Dictionary<IIncomingCall, IConferenceDeviceControl>();
			m_SubscribedConferenceControls = new List<IConferenceDeviceControl>();
			m_IncomingCallsSection = new SafeCriticalSection();
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcIncomingCallView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IIncomingCall incomingCall = GetFirstIncomingCall();
				string info = GetCallerInfo(incomingCall);

				view.SetCallerInfo(info);

				view.PlayRingtone(incomingCall != null);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private static string GetCallerInfo(IIncomingCall incomingCall)
		{
			string output = string.Empty;

			if (incomingCall == null)
				return output;

			string name = string.IsNullOrEmpty(incomingCall.Name) ? "Unknown" : incomingCall.Name.Trim();
			string number = string.IsNullOrEmpty(incomingCall.Number) ? "Unknown" : incomingCall.Number.Trim();

			return number == name ? name : string.Format("{0} - {1}", name, number);
		}

		/// <summary>
		/// Gets the first unanswered call.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IIncomingCall GetFirstIncomingCall()
		{
			return m_IncomingCallsSection.Execute(() => m_IncomingCalls.Select(kvp => kvp.Key).FirstOrDefault());
		}

		/// <summary>
		/// Gets the conference controls.
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<IConferenceDeviceControl> GetConferenceControls(IConnectProRoom room)
		{
			return room.ConferenceManager.GetDialingProviders();
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

			ShowView(IncomingCallCount > 0);
			RefreshIfVisible();
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
				if (!m_IncomingCalls.Remove(call))
					return;

				Unsubscribe(call);
			}
			finally
			{
				m_IncomingCallsSection.Leave();
			}

			Refresh();
			ShowView(IncomingCallCount > 0);
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIncomingCallAnswered += RoomOnIncomingCallAnswered;
			room.OnIncomingCallRejected += RoomOnIncomingCallRejected;

			m_SubscribedConferenceControls.Clear();

			foreach (IConferenceDeviceControl dialer in GetConferenceControls(room))
			{
				m_SubscribedConferenceControls.Add(dialer);

				dialer.OnIncomingCallAdded += VideoDialerOnIncomingCallAdded;
				dialer.OnIncomingCallRemoved += VideoDialerOnIncomingCallRemoved;
			}
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIncomingCallAnswered -= RoomOnIncomingCallAnswered;
			room.OnIncomingCallRejected -= RoomOnIncomingCallRejected;

			foreach (IConferenceDeviceControl dialer in m_SubscribedConferenceControls)
			{
				dialer.OnIncomingCallAdded -= VideoDialerOnIncomingCallAdded;
				dialer.OnIncomingCallRemoved -= VideoDialerOnIncomingCallRemoved;
			}

			m_SubscribedConferenceControls.Clear();
		}

		private void RoomOnIncomingCallAnswered(object sender, GenericEventArgs<IIncomingCall> eventArgs)
		{
			RemoveIncomingCall(eventArgs.Data);
		}

		private void RoomOnIncomingCallRejected(object sender, GenericEventArgs<IIncomingCall> eventArgs)
		{
			RemoveIncomingCall(eventArgs.Data);
		}

		/// <summary>
		/// Called when a new incoming call is added.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VideoDialerOnIncomingCallAdded(object sender, GenericEventArgs<IIncomingCall> args)
		{
			IIncomingCall call = args.Data;
			if (call.GetIsRingingIncomingCall())
				AddIncomingCall(call, sender as IConferenceDeviceControl);
		}

		/// <summary>
		/// Called when an incoming call is removed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VideoDialerOnIncomingCallRemoved(object sender, GenericEventArgs<IIncomingCall> args)
		{
			RemoveIncomingCall(args.Data);
		}

		#endregion

		#region Incoming Call Callbacks

		/// <summary>
		/// Subscribe to the call events.
		/// </summary>
		/// <param name="call"></param>
		private void Subscribe(IIncomingCall call)
		{
			call.OnNameChanged += SourceOnNameChanged;
			call.OnNumberChanged += SourceOnNumberChanged;
			call.OnAnswerStateChanged += SourceOnStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the call events.
		/// </summary>
		/// <param name="call"></param>
		private void Unsubscribe(IIncomingCall call)
		{
			call.OnNameChanged -= SourceOnNameChanged;
			call.OnNumberChanged -= SourceOnNumberChanged;
			call.OnAnswerStateChanged -= SourceOnStatusChanged;
		}

		/// <summary>
		/// Called when the call status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnStatusChanged(object sender, IncomingCallAnswerStateEventArgs args)
		{
			IIncomingCall call = sender as IIncomingCall;
			if (call == null)
				return;

			if (!call.GetIsRingingIncomingCall())
				RemoveIncomingCall(call);
		}

		/// <summary>
		/// Called when the call number changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnNumberChanged(object sender, StringEventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the call name changes.
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
		protected override void Subscribe(IVtcIncomingCallView view)
		{
			base.Subscribe(view);

			view.OnAnswerButtonPressed += ViewOnAnswerButtonPressed;
			view.OnIgnoreButtonPressed += ViewOnIgnoreButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcIncomingCallView view)
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
			IIncomingCall call = GetFirstIncomingCall();

			if (call != null && Room != null)
				Room.RejectIncomingCall(call);

			ShowView(false);
		}

		/// <summary>
		/// Called when the user presses the answer button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnAnswerButtonPressed(object sender, EventArgs e)
		{
			IIncomingCall call = GetFirstIncomingCall();
			IConferenceDeviceControl control =
				call == null
					? null
					: m_IncomingCallsSection.Execute(() => m_IncomingCalls.GetDefault(call));

			if (call != null && Room != null)
				Room.AnswerIncomingCall(control, call);

			ShowView(false);
		}

		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			GetView().PlayRingtone(false);
		}

		#endregion
	}
}
