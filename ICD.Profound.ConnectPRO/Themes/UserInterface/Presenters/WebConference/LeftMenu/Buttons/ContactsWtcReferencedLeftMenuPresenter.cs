using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(IContactsWtcReferencedLeftMenuPresenter))]
	public sealed class ContactsWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                             IContactsWtcReferencedLeftMenuPresenter
	{
		private readonly IWtcContactListPresenter m_ContactListPresenter;

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
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_ContactListPresenter);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedLeftMenuView view)
		{
			Icon = "list";
			Label = "Contacts";
			State = null;
			Enabled = true;
			Selected = m_ContactListPresenter.IsViewVisible;

			base.Refresh(view);
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
			RefreshIfVisible();
		}

		#endregion

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			Navigation.NavigateTo<IWtcContactListPresenter>();
		}
	}
}
