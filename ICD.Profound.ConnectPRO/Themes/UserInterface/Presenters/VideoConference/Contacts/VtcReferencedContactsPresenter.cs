using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public sealed class VtcReferencedContactsPresenter : AbstractComponentPresenter<IVtcReferencedContactsView>,
	                                                     IVtcReferencedContactsPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcReferencedContactsPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		#region View Callbacks

		protected override void Subscribe(IVtcReferencedContactsView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
			view.OnFavoriteButtonPressed += ViewOnFavoriteButtonPressed;
		}

		protected override void Unsubscribe(IVtcReferencedContactsView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
			view.OnFavoriteButtonPressed -= ViewOnFavoriteButtonPressed;
		}

		private void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
