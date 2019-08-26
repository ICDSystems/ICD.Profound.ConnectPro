using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions
{
	[PresenterBinding(typeof(IFloatingActionCameraPresenter))]
	public sealed class FloatingActionCameraPresenter : AbstractFloatingActionPresenter<IFloatingActionCameraView>, IFloatingActionCameraPresenter
	{
		private readonly ICameraControlPresenter m_CameraControl;
        private readonly ICameraActivePresenter m_CameraActive;

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
			m_CameraControl = Navigation.LazyLoadPresenter<ICameraControlPresenter>();
			Subscribe(m_CameraControl);

            m_CameraActive = Navigation.LazyLoadPresenter<ICameraActivePresenter>();
            Subscribe(m_CameraActive);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_CameraControl);
            Unsubscribe(m_CameraActive);
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetActive()
		{
			return m_CameraControl.IsViewVisible || m_CameraActive.IsViewVisible;
		}

		#region View Callbacks

		/// <summary>
		/// Called when the user presses the option button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
            if (m_CameraControl.IsViewVisible || m_CameraActive.IsViewVisible)
            {
			    m_CameraControl.ShowView(false);
                m_CameraActive.ShowView(false);
            }
            else if (m_CameraControl.CameraCount > 0)
                m_CameraControl.ShowView(true);
            else if (m_CameraActive.CameraCount > 1)
                m_CameraActive.ShowView(true);
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
            {
				m_CameraControl.ShowView(false);
                m_CameraActive.ShowView(false);
            }
		}

		#endregion

		#region Navigation Callbacks

		/// <summary>
		/// Subscribe to the camera control menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Subscribe(ICameraControlPresenter menu)
		{
			menu.OnViewVisibilityChanged += MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the camera control menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Unsubscribe(ICameraControlPresenter menu)
		{
			menu.OnViewVisibilityChanged -= MenuOnViewVisibilityChanged;
		}
        
        /// <summary>
		/// Subscribe to the camera active menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Subscribe(ICameraActivePresenter menu)
		{
			menu.OnViewVisibilityChanged += MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the camera active menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Unsubscribe(ICameraActivePresenter menu)
		{
			menu.OnViewVisibilityChanged -= MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when the menu visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void MenuOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
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
			ShowView(callEventArgs.Data == eInCall.Video && (m_CameraControl.CameraCount > 0 || m_CameraActive.CameraCount > 1));
		}

		#endregion
	}
}
