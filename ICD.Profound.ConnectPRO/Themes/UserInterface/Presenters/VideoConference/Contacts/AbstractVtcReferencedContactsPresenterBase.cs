using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public abstract class AbstractVtcReferencedContactsPresenterBase : AbstractUiComponentPresenter<IVtcReferencedContactsView>,
	                                                               IVtcReferencedContactsPresenterBase
	{
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;
		private bool m_Selected;

		public virtual bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				RefreshIfVisible();
			}
		}

		public bool HideFavoriteIcon { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractVtcReferencedContactsPresenterBase(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcReferencedContactsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string name = GetName();
				bool favorite = GetIsFavorite();
				bool favoriteVisible = !HideFavoriteIcon && GetIsFavoriteVisible();

				view.SetContactName(name);
				view.SetIsFavorite(favorite);
				view.SetFavoriteButtonVisible(favoriteVisible);
				view.SetSelected(Selected);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Override to control the display name for this contact.
		/// </summary>
		/// <returns></returns>
		protected abstract string GetName();

		/// <summary>
		/// Override to control the favorite state for this contact.
		/// </summary>
		/// <returns></returns>
		protected abstract bool GetIsFavorite();

		/// <summary>
		/// Override to control the visibility of the favorite button for this contact.
		/// </summary>
		/// <returns></returns>
		protected abstract bool GetIsFavoriteVisible();

		void IVtcReferencedContactsPresenterBase.SetModel(object model)
		{
			SetModel(model);
		}

		void IVtcReferencedContactsPresenterBase.Dial()
		{
			Dial();
		}

		protected abstract void SetModel(object model);

		protected abstract void Dial();

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

		protected virtual void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion

		public ITraditionalConferenceDeviceControl ActiveConferenceControl { set; protected get; }
	}
}
