using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions
{
	[PresenterBinding(typeof(IFloatingActionCameraPresenter))]
	public sealed class FloatingActionCameraPresenter : AbstractFloatingActionPresenter<IFloatingActionCameraView>, IFloatingActionCameraPresenter
	{
		private readonly ICameraButtonsPresenter m_Buttons;

		private IConferenceManager m_SubscribedConferenceManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public FloatingActionCameraPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Buttons = Navigation.LazyLoadPresenter<ICameraButtonsPresenter>();
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetActive()
		{
			return m_Buttons.IsViewVisible;
		}

		#region View Callbacks

		/// <summary>
		/// Called when the user presses the option button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Buttons.ShowView(!m_Buttons.IsViewVisible);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (!args.Data)
				m_Buttons.ShowView(false);
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

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnInCallChanged += SubscribedConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedConferenceManager == null)
				return;
			
			m_SubscribedConferenceManager.OnInCallChanged -= SubscribedConferenceManagerOnInCallChanged;
		}

		private void SubscribedConferenceManagerOnInCallChanged(object sender, InCallEventArgs callEventArgs)
		{
			ShowView(callEventArgs.Data == eInCall.Video);
		}

		#endregion
	}
}
