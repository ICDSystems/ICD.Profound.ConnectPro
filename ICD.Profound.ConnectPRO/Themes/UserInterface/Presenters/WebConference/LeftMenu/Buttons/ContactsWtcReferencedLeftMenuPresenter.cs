using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(IContactsWtcReferencedLeftMenuPresenter))]
	public sealed class ContactsWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                             IContactsWtcReferencedLeftMenuPresenter
	{
		private IWtcContactListPresenter m_ContactListPresenter;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ContactsWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                              ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_ContactListPresenter = Navigation.LazyLoadPresenter<IWtcContactListPresenter>();
			Subscribe(m_ContactListPresenter);

			Icon = "list";
			Label = "Contacts";
			State = null;
			Enabled = true;
		}

		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_ContactListPresenter);
		}

		#region Presenter Callbacks

		private void Subscribe(IWtcContactListPresenter contactListPresenter)
		{
			contactListPresenter.OnViewVisibilityChanged += ContactListPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(IWtcContactListPresenter contactListPresenter)
		{
			contactListPresenter.OnViewVisibilityChanged -= ContactListPresenterOnViewVisibilityChanged;
		}

		private void ContactListPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			Selected = m_ContactListPresenter.IsViewVisible;
		}

		#endregion

		//#region Conference Control Callbacks

		//protected override void Subscribe(IWebConferenceDeviceControl control)
		//{
		//	if (control == null)
		//		return;

		//	base.Subscribe(control);

		//	control.OnConferenceAdded += ControlOnConferenceAdded;
		//}

		//protected override void Unsubscribe(IWebConferenceDeviceControl control)
		//{
		//	if (control == null)
		//		return;

		//	base.Unsubscribe(control);

		//	control.OnConferenceAdded -= ControlOnConferenceAdded;
		//}

		//private void ControlOnConferenceAdded(object sender, ConferenceEventArgs e)
		//{
		//	Enabled = e.Data.Status == eConferenceStatus.Connected;
		//}

		//#endregion

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			Navigation.NavigateTo<IWtcContactListPresenter>();
		}
	}
}
