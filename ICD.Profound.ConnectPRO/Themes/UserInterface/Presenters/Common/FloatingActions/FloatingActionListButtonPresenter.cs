using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions.FloatingActionListItems;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions.FloatingActionListItems;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions
{
	[PresenterBinding(typeof(IFloatingActionListButtonPresenter))]
	public sealed class FloatingActionListButtonPresenter : AbstractFloatingActionPresenter<IFloatingActionListButtonView>,
	                                                        IFloatingActionListButtonPresenter
	{
		private const string LIST_ICON = "list";

		private readonly List<IFloatingActionListItem> m_ListItems;

		private IFloatingActionListItem m_ShortcutAction;

		private readonly IFloatingActionListPresenter m_ListPresenter;

		private IFloatingActionListItem ShortcutAction
		{
			get { return m_ShortcutAction; }
			set
			{
				if (value == m_ShortcutAction)
					return;

				UnsubscribeShortcut(m_ShortcutAction);
				m_ShortcutAction = value;
				SubscribeShortcut(value);
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public FloatingActionListButtonPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                         ConnectProTheme theme) : base(nav, views, theme)
		{
			m_ListItems = new List<IFloatingActionListItem>()
			{
				new LightsListItem(Navigation)
			};

			foreach (IFloatingActionListItem item in m_ListItems)
			{
				Subscribe(item);
			}

			m_ListPresenter = Navigation.LazyLoadPresenter<IFloatingActionListPresenter>();

			Subscribe(m_ListPresenter);

			m_ListPresenter.SetChildren(m_ListItems);

			UpdateVisibilityAndRefresh();
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetActive()
		{
			return ShortcutAction != null ? ShortcutAction.IsActive : m_ListPresenter.IsViewVisible;
		}

		/// <summary>
		/// Override to get the enabled state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetEnabled()
		{
			return true;
		}

		/// <summary>
		/// Override to handle the button press.
		/// </summary>
		protected override void HandleButtonPress()
		{

			if (ShortcutAction != null)
				// If there is a Shortcut Action, do that
				ShortcutAction.HandleButtonPressed();
			else
				// If no Shortcut Action, toggle the list view
				m_ListPresenter.ShowView(!m_ListPresenter.IsViewVisible);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IFloatingActionListButtonView view)
		{
			int count = m_ListItems.Count(i => i.IsAvailable);

			switch (count)
			{
				//We shouldn't get here, but we'll condition it out anyway
				case 0:
					ShortcutAction = null;
					ShowView(false);
					return;

				//If only one item is visible, set it as the direct action
				case 1:
					ShortcutAction = m_ListItems.First(i => i.IsAvailable);
					view.SetIcon(GetWhiteIconString(ShortcutAction.Icon));

					// If the List is visible, hide it now
					m_ListPresenter.ShowView(false);
					break;

				default:
					ShortcutAction = null;
					view.SetIcon(GetWhiteIconString(LIST_ICON));
					break;
			}

			base.Refresh(view);
		}

		private void UpdateVisibilityAndRefresh()
		{
			bool refreshNeeded = UpdateVisibility();

			if (refreshNeeded)
				RefreshIfVisible();
		}

		/// <summary>
		/// Updates the visibility of the view, based on the list items
		/// </summary>
		/// <returns>Returns true if view is and was visible, and should be refreshed now</returns>
		private bool UpdateVisibility()
		{

			bool visibility = m_ListItems.Any(i => i.IsAvailable);
			bool wasVisible = IsViewVisible;

			ShowView(visibility);

			if (!visibility)
			{
				//If the button isn't visible any more, we should hide the list too
				m_ListPresenter.ShowView(false);
			}

			return visibility && wasVisible;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			ShortcutAction = null;

			foreach (IFloatingActionListItem item in m_ListItems)
			{
				Unsubscribe(item);
				item.Dispose();
			}

			m_ListItems.Clear();
		}

		#region ListItem Callbacks

		private void Subscribe(IFloatingActionListItem item)
		{
			if (item == null)
				return;

			item.OnIsAvaliableChanged += ItemOnIsAvaliableChanged;
		}

		private void Unsubscribe(IFloatingActionListItem item)
		{
			if (item == null)
				return;

			item.OnIsAvaliableChanged -= ItemOnIsAvaliableChanged;
		}

		private void ItemOnIsAvaliableChanged(object sender, BoolEventArgs e)
		{
			UpdateVisibilityAndRefresh();
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

			m_ListItems.ForEach(i => i.Subscribe(room));

			UpdateVisibilityAndRefresh();
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			m_ListItems.ForEach(i => i.Unsubscribe(room));

			UpdateVisibilityAndRefresh();
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			m_ListItems.ForEach(i => i.SetRoom(room));

			UpdateVisibilityAndRefresh();
		}

		#endregion

		#region Shortcut List Item Callbacks

		private void SubscribeShortcut(IFloatingActionListItem shortcut)
		{
			if (shortcut == null)
				return;

			shortcut.OnIsActiveChanged += ShortcutOnIsActiveChanged;
		}

		private void UnsubscribeShortcut(IFloatingActionListItem shortcut)
		{
			if (shortcut == null)
				return;

			shortcut.OnIsActiveChanged -= ShortcutOnIsActiveChanged;
		}

		private void ShortcutOnIsActiveChanged(object sender, BoolEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion

		#region IFloatingActionListPresenter Callbacks

		private void Subscribe(IFloatingActionListPresenter listPresenter)
		{
			if (listPresenter == null)
				return;

			listPresenter.OnViewVisibilityChanged += ListPresenterOnViewVisibilityChanged;
		}

		private void ListPresenterOnViewVisibilityChanged(object sender, BoolEventArgs args)
		{
			//Refresh if Shortcut action is null
			if (ShortcutAction == null)
				RefreshIfVisible();
		}

		#endregion

		private static string GetWhiteIconString(string icon)
		{
			return Icons.GetIcon(icon, eIconColor.White);
		}
	}
}
