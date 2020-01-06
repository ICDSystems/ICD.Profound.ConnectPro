using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions.FloatingActionListItems;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions.FloatingActionListItems
{
	public abstract class AbstractFloatingActionListItem : IFloatingActionListItem
	{
		private bool m_IsAvaliable;

		private bool m_IsActive;

		private readonly INavigationController m_Navigation;

		/// <summary>
		/// This is the presenter for the page this list item shows
		/// </summary>
		protected abstract IUiPresenter ActionPresenter { get; }

		protected bool IsActive
		{
			get { return m_IsActive; }
			set
			{
				if (m_IsActive == value)
					return;

				m_IsActive = value;

				OnIsActiveChanged.Raise(this, new BoolEventArgs(value));
			}
		}

		protected INavigationController Navigation { get { return m_Navigation; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractFloatingActionListItem(INavigationController navigation)
		{
			m_Navigation = navigation;
		}

		public event EventHandler<BoolEventArgs> OnIsAvaliableChanged;

		public event EventHandler<BoolEventArgs> OnIsActiveChanged;

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
			return IsActive;
		}

		/// <summary>
		/// This method will get called when the button is pressed - also called by the FloatingActionListButtonPresenter if this is the only icon
		/// </summary>
		public virtual void HandleButtonPressed()
		{
			if (ActionPresenter != null)
				ActionPresenter.ShowView(!ActionPresenter.IsViewVisible);
		}

		public virtual void Subscribe(IConnectProRoom room)
		{
		}

		public virtual void Unsubscribe(IConnectProRoom room)
		{
		}

		#region ActionPresenter Callbacks

		protected virtual void SubscribeActionPresenter(IUiPresenter actionPresenter)
		{
			if (actionPresenter == null)
				return;

			actionPresenter.OnViewVisibilityChanged += ActionPresenterOnViewVisibilityChanged;
		}

		protected virtual void ActionPresenterOnViewVisibilityChanged(object sender, BoolEventArgs args)
		{
			IsActive = args.Data;
		}

		#endregion

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			OnIsActiveChanged = null;
			OnIsActiveChanged = null;
		}
	}
}
