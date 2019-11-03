using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	[PresenterBinding(typeof(IWtcCallOutPresenter))]
	public sealed class WtcCallOutPresenter : AbstractWtcPresenter<IWtcCallOutView>, IWtcCallOutPresenter
	{
		private readonly KeypadStringBuilder m_StringBuilder;
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Gets the zoom traditional control for call out.
		/// </summary>
		public ZoomRoomTraditionalConferenceControl TraditionalControl
		{
			get { return GetTraditionalConferenceControl(ActiveConferenceControl); }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcCallOutPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_RefreshSection = new SafeCriticalSection();

			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;
		}

		protected override void Refresh(IWtcCallOutView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string dialText = m_StringBuilder.ToString();
				string callLabel = "CALL";
				bool backEnabled = dialText.Length > 0;
				bool callSelected = false;
				bool clearEnabled = dialText.Length > 0;

				view.SetBackButtonEnabled(backEnabled);
				view.SetCallButtonLabel(callLabel);
				view.SetCallButtonSelected(callSelected);
				view.SetClearButtonEnabled(clearEnabled);
				view.SetText(dialText);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		/// <summary>
		/// Gets the zoom traditional control for call out from the given conference control.
		/// </summary>
		[CanBeNull]
		private static ZoomRoomTraditionalConferenceControl GetTraditionalConferenceControl(
			[CanBeNull] IWebConferenceDeviceControl control)
		{
			if (control == null)
				return null;

			ZoomRoom device = control.Parent as ZoomRoom;
			return device == null ? null : device.Controls.GetControl<ZoomRoomTraditionalConferenceControl>();
		}

		/// <summary>
		/// Called when the string builder is updated.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StringBuilderOnStringChanged(object sender, StringEventArgs eventArgs)
		{
			RefreshIfVisible();
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

			ZoomRoomTraditionalConferenceControl traditionalControl = GetTraditionalConferenceControl(control);
			if (traditionalControl == null)
				return;

			traditionalControl.OnConferenceAdded += TraditionalControlOnConferenceAdded;
			traditionalControl.OnConferenceRemoved += TraditionalControlOnConferenceRemoved;
		}

		/// <summary>
		/// Unsusbcribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			ZoomRoomTraditionalConferenceControl traditionalControl = GetTraditionalConferenceControl(control);
			if (traditionalControl == null)
				return;

			traditionalControl.OnConferenceAdded -= TraditionalControlOnConferenceAdded;
			traditionalControl.OnConferenceRemoved -= TraditionalControlOnConferenceRemoved;
		}

		/// <summary>
		/// Called when an active conference starts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="conferenceEventArgs"></param>
		private void TraditionalControlOnConferenceAdded(object sender, ConferenceEventArgs conferenceEventArgs)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Called when the active conference ends.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="conferenceEventArgs"></param>
		private void TraditionalControlOnConferenceRemoved(object sender, ConferenceEventArgs conferenceEventArgs)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IWtcCallOutView view)
		{
			base.Subscribe(view);

			view.OnBackButtonPressed += ViewOnBackButtonPressed;
			view.OnCallButtonPressed += ViewOnCallButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWtcCallOutView view)
		{
			base.Unsubscribe(view);

			view.OnBackButtonPressed -= ViewOnBackButtonPressed;
			view.OnCallButtonPressed -= ViewOnCallButtonPressed;
			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="charEventArgs"></param>
		private void ViewOnKeypadButtonPressed(object sender, CharEventArgs charEventArgs)
		{
			m_StringBuilder.AppendCharacter(charEventArgs.Data);
		}

		/// <summary>
		/// Called when the user presses the clear button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnClearButtonPressed(object sender, EventArgs eventArgs)
		{
			m_StringBuilder.Clear();
		}

		/// <summary>
		/// Called when the user presses the call button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCallButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Called when the user presses the back button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnBackButtonPressed(object sender, EventArgs eventArgs)
		{
			m_StringBuilder.Backspace();
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_StringBuilder.Clear();
		}

		#endregion
	}
}