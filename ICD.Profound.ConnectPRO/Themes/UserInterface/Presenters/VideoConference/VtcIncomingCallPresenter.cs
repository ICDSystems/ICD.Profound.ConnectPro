using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
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

			room.Dialing.OnIncomingCallAnswered += RoomOnIncomingCallAnswered;
			room.Dialing.OnIncomingCallRejected += RoomOnIncomingCallRejected;

			if (room.ConferenceManager == null)
				return;

			room.ConferenceManager.Dialers.OnIncomingCallAdded += ConferenceManagerOnIncomingCallAdded;
			room.ConferenceManager.Dialers.OnIncomingCallRemoved += ConferenceManagerOnIncomingCallRemoved;
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

			room.Dialing.OnIncomingCallAnswered -= RoomOnIncomingCallAnswered;
			room.Dialing.OnIncomingCallRejected -= RoomOnIncomingCallRejected;

			if (room.ConferenceManager == null)
				return;

			room.ConferenceManager.Dialers.OnIncomingCallAdded -= ConferenceManagerOnIncomingCallAdded;
			room.ConferenceManager.Dialers.OnIncomingCallRemoved -= ConferenceManagerOnIncomingCallRemoved;
		}

		/// <summary>
		/// Called when the room answers the incoming call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnIncomingCallAnswered(object sender, GenericEventArgs<IIncomingCall> eventArgs)
		{
			RemoveIncomingCall(eventArgs.Data);
		}

		/// <summary>
		/// Called when the room rejects the incoming call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnIncomingCallRejected(object sender, GenericEventArgs<IIncomingCall> eventArgs)
		{
			RemoveIncomingCall(eventArgs.Data);
		}

		/// <summary>
		/// Called when a new incoming call is added.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ConferenceManagerOnIncomingCallAdded(object sender, ConferenceControlIncomingCallEventArgs eventArgs)
		{
			IIncomingCall call = eventArgs.IncomingCall;
			if (call.GetIsRingingIncomingCall())
				AddIncomingCall(call, eventArgs.Control);
		}

		/// <summary>
		/// Called when an incoming call is removed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ConferenceManagerOnIncomingCallRemoved(object sender, ConferenceControlIncomingCallEventArgs eventArgs)
		{
			RemoveIncomingCall(eventArgs.IncomingCall);
		}

		#endregion

		#region Incoming Call Callbacks

		/// <summary>
		/// Subscribe to the call events.
		/// </summary>
		/// <param name="call"></param>
		private void Subscribe(IIncomingCall call)
		{
			call.OnNameChanged += CallOnNameChanged;
			call.OnNumberChanged += CallOnNumberChanged;
			call.OnAnswerStateChanged += CallOnStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the call events.
		/// </summary>
		/// <param name="call"></param>
		private void Unsubscribe(IIncomingCall call)
		{
			call.OnNameChanged -= CallOnNameChanged;
			call.OnNumberChanged -= CallOnNumberChanged;
			call.OnAnswerStateChanged -= CallOnStatusChanged;
		}

		/// <summary>
		/// Called when the call status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CallOnStatusChanged(object sender, IncomingCallAnswerStateEventArgs args)
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
				Room.Dialing.RejectIncomingCall(call);

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
				Room.Dialing.AnswerIncomingCall(control, call);

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
