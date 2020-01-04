using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions.FloatingActionListItems;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions.FloatingActionListItems;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions.FloatingActionListItems
{
	public abstract class AbstractFloatingActionListItemPresenter<T> : AbstractUiPresenter<T>, IFloatingActionListItemPresenter<T> where T : class, IFloatingActionListItemView
	{
		private bool m_IsAvaliable;

		/// <summary>
		/// This is the presenter for the page this list item shows
		/// </summary>
		protected abstract IUiPresenter ActionPresenter { get; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractFloatingActionListItemPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		public event EventHandler<BoolEventArgs> OnIsAvaliableChanged;

		public bool IsAvailable
		{
			get { return m_IsAvaliable; }
			protected set
			{
				if (m_IsAvaliable == value)
					return;

				m_IsAvaliable = value;

				OnIsAvaliableChanged.Raise(this, new BoolEventArgs(value));
			}
		}

		public abstract string Label { get; }

		public abstract string Icon { get; }

		public virtual bool GetActive()
		{
			return ActionPresenter != null && ActionPresenter.IsViewVisible;
		}

		/// <summary>
		/// This method will get called when the button is pressed - also called by the FloatingActionListButtonPresenter if this is the only icon
		/// </summary>
		public virtual void HandleButtonPressed()
		{
			if (ActionPresenter != null)
				ActionPresenter.ShowView(!ActionPresenter.IsViewVisible);
		}
	}
}
