using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.Conferencing.Polycom.Devices.Codec;
using ICD.Connect.Conferencing.Polycom.Devices.Codec.Components.Button;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public sealed class VtcContactsPolycomPresenter : AbstractVtcContactsPresenter<IVtcContactsPolycomView>, IVtcContactsPolycomPresenter
	{
		private enum ePolycomDirectoryMode
		{
			Navigation,
			Local,
			Recents
		}

		private readonly SafeCriticalSection m_RefreshSection;

		private ePolycomDirectoryMode m_DirectoryMode;

		private ButtonComponent m_ButtonComponent;

		#region Properties

		/// <summary>
		/// Gets/sets the directory mode for populating the contacts list.
		/// </summary>
		private ePolycomDirectoryMode DirectoryMode
		{
			get { return m_DirectoryMode; }
			set
			{
				if (value == m_DirectoryMode)
					return;

				m_DirectoryMode = value;
				Selected = null;

				Refresh();
			}
		}

		protected override bool HideFavoriteIcons { get { return true; } }

		protected override bool CallButtonEnabled { get { return m_DirectoryMode == ePolycomDirectoryMode.Navigation || base.CallButtonEnabled; } }

		protected override bool HangupButtonEnabled { get { return m_DirectoryMode == ePolycomDirectoryMode.Navigation || base.HangupButtonEnabled; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcContactsPolycomPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Refresh the visual state of the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcContactsPolycomView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetDPadVisible(m_DirectoryMode == ePolycomDirectoryMode.Navigation);

				view.SetDPadButtonSelected(m_DirectoryMode == ePolycomDirectoryMode.Navigation);
				view.SetLocalButtonSelected(m_DirectoryMode == ePolycomDirectoryMode.Local);
				view.SetRecentButtonSelected(m_DirectoryMode == ePolycomDirectoryMode.Recents);

				view.SetBackButtonVisible(m_DirectoryMode == ePolycomDirectoryMode.Local ||
				                          m_DirectoryMode == ePolycomDirectoryMode.Navigation);
				view.SetHomeButtonVisible(m_DirectoryMode == ePolycomDirectoryMode.Local ||
				                          m_DirectoryMode == ePolycomDirectoryMode.Navigation);
				view.SetDirectoryButtonVisible(m_DirectoryMode == ePolycomDirectoryMode.Navigation);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		protected override IEnumerable<ModelPresenterTypeInfo> GetContacts()
		{
			return GetContacts(m_DirectoryMode);
		}

		private IEnumerable<ModelPresenterTypeInfo> GetContacts(ePolycomDirectoryMode directoryMode)
		{
			switch (directoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					return Enumerable.Empty<ModelPresenterTypeInfo>();

				case ePolycomDirectoryMode.Local:
					if (DirectoryBrowser == null)
						return Enumerable.Empty<ModelPresenterTypeInfo>();

					IDirectoryFolder current = DirectoryBrowser.GetCurrentFolder();
					if (current == null)
						return Enumerable.Empty<ModelPresenterTypeInfo>();

					return current
						.GetFolders()
						.Cast<object>()
						.Concat(current.GetContacts())
						.OrderBy(c => c is IContact)
						.ThenBy(c =>
						        {
							        if (c is IContact)
								        return (c as IContact).Name;
									if (c is IDirectoryFolder)
										return (c as IDirectoryFolder).Name;

									// This should never happen
									throw new InvalidOperationException();
						        })
						.Select(c =>
						        {
							        ModelPresenterTypeInfo.ePresenterType type = (c is IDirectoryFolder)
								                                                     ? ModelPresenterTypeInfo
									                                                       .ePresenterType.Folder
								                                                     : ModelPresenterTypeInfo
									                                                       .ePresenterType.Contact;
							        return new ModelPresenterTypeInfo(type, c);
						        });
				
				case ePolycomDirectoryMode.Recents:
					return
						ConferenceManager == null
							? Enumerable.Empty<ModelPresenterTypeInfo>()
							: ConferenceManager
								.GetRecentSources()
								.Reverse()
								.Distinct()
								.Select(c => new ModelPresenterTypeInfo(ModelPresenterTypeInfo.ePresenterType.Recent, c));

				default:
					throw new ArgumentOutOfRangeException("directoryMode");
			}
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

			
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			m_ButtonComponent = null;
		}

		#endregion

		#region Conference Control Callbacks

		protected override void Subscribe(ITraditionalConferenceDeviceControl control)
		{
			base.Subscribe(control);

			PolycomGroupSeriesDevice videoDialerDevice = control == null
															 ? null
															 : control.Parent as PolycomGroupSeriesDevice;
			m_ButtonComponent = videoDialerDevice == null ? null : videoDialerDevice.Components.GetComponent<ButtonComponent>();
		}

		protected override void Unsubscribe(ITraditionalConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			m_ButtonComponent = null;
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcContactsPolycomView view)
		{
			base.Subscribe(view);

			view.OnNavigationButtonPressed += ViewOnNavigationButtonPressed;
			view.OnLocalButtonPressed += ViewOnLocalButtonPressed;

			view.OnDPadDownButtonPressed += ViewOnDPadDownButtonPressed;
			view.OnDPadLeftButtonPressed += ViewOnDPadLeftButtonPressed;
			view.OnDPadRightButtonPressed += ViewOnDPadRightButtonPressed;
			view.OnDPadUpButtonPressed += ViewOnDPadUpButtonPressed;
			view.OnDPadSelectButtonPressed += ViewOnDPadSelectButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcContactsPolycomView view)
		{
			base.Unsubscribe(view);

			view.OnNavigationButtonPressed -= ViewOnNavigationButtonPressed;
			view.OnLocalButtonPressed -= ViewOnLocalButtonPressed;

			view.OnDPadDownButtonPressed -= ViewOnDPadDownButtonPressed;
			view.OnDPadLeftButtonPressed -= ViewOnDPadLeftButtonPressed;
			view.OnDPadRightButtonPressed -= ViewOnDPadRightButtonPressed;
			view.OnDPadUpButtonPressed -= ViewOnDPadUpButtonPressed;
			view.OnDPadSelectButtonPressed -= ViewOnDPadSelectButtonPressed;
		}

		private void ViewOnDPadSelectButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Select);
		}

		private void ViewOnDPadUpButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Up);
		}

		private void ViewOnDPadRightButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Right);
		}

		private void ViewOnDPadLeftButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Left);
		}

		private void ViewOnDPadDownButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Down);
		}

		/// <summary>
		/// Called when the user presses the search button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnDirectoryButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eMisc.Directory);
					break;
			}
		}

		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnHomeButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eMisc.Home);
					break;

				case ePolycomDirectoryMode.Local:
					base.ViewOnHomeButtonPressed(sender, eventArgs);
					break;
			}
		}

		/// <summary>
		/// Called when the user presses the back button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnBackButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eCall.Back);
					break;

				case ePolycomDirectoryMode.Local:
					base.ViewOnBackButtonPressed(sender, eventArgs);
					break;
			}
		}

		/// <summary>
		/// Called when the user presses the recents button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnRecentButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = ePolycomDirectoryMode.Recents;
		}

		/// <summary>
		/// Called when the user presses the local button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnLocalButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = ePolycomDirectoryMode.Local;
		}

		/// <summary>
		/// Called when the user presses the dpad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnNavigationButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = ePolycomDirectoryMode.Navigation;
		}

		/// <summary>
		/// Called when the user presses the call button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnCallButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eCall.Call);
					break;

				case ePolycomDirectoryMode.Local:
				case ePolycomDirectoryMode.Recents:
					base.ViewOnCallButtonPressed(sender, eventArgs);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Called when the user presses the hangup button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eCall.Hangup);
					break;

				case ePolycomDirectoryMode.Local:
				case ePolycomDirectoryMode.Recents:
					base.ViewOnHangupButtonPressed(sender, eventArgs);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion
	}
}
