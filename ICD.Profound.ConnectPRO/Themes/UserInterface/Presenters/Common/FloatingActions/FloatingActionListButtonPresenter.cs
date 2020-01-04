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

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions
{
	[PresenterBinding(typeof(IFloatingActionListButtonPresenter))]
	public sealed class FloatingActionListButtonPresenter : AbstractFloatingActionPresenter<IFloatingActionListButtonView>,
	                                                        IFloatingActionListButtonPresenter
	{
		private const string LIST_ICON = "list";

		private readonly List<IFloatingActionListItemPresenter> m_ListItems;

		private IFloatingActionListItemPresenter m_ShortcutAction;

		private readonly IFloatingActionListPresenter m_ListPresenter;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public FloatingActionListButtonPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                         ConnectProTheme theme) : base(nav, views, theme)
		{
			m_ListItems = new List<IFloatingActionListItemPresenter>()
			{
				new LightsItemPresenter(nav, views, theme)
			};

			foreach (IFloatingActionListItemPresenter item in m_ListItems)
			{
				Subscibe(item);
			}

			m_ListPresenter = Navigation.LazyLoadPresenter<IFloatingActionListPresenter>();

			m_ListPresenter.SetChildren(m_ListItems);
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetActive()
		{
			if (m_ShortcutAction != null)
				return m_ShortcutAction.GetActive();

			return m_ListPresenter.IsViewVisible;
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

			if (m_ShortcutAction != null)
				// If there is a Shortcut Action, do that
				m_ShortcutAction.HandleButtonPressed();
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
			base.Refresh(view);

			int count = m_ListItems.Count(i => i.IsAvailable);

			switch (count)
			{
				//We shouldn't get here, but we'll condition it out anyway
				case 0:
					m_ShortcutAction = null;
					ShowView(false);
					return;

				//If only one item is visible, set it as the direct action
				case 1:
					m_ShortcutAction = m_ListItems.First(i => i.IsAvailable);
					view.SetIcon(GetWhiteIconString(m_ShortcutAction.Icon));

					// If the List is visible, hide it now
					m_ListPresenter.ShowView(false);
					break;

				default:
					m_ShortcutAction = null;
					view.SetIcon(GetWhiteIconString(LIST_ICON));
					break;
			}
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

		private static string GetWhiteIconString(string icon)
		{
			return Icons.GetIcon(icon, eIconColor.White);
		}

		#region ListItem Callbacks

		private void Subscibe(IFloatingActionListItemPresenter item)
		{
			if (item == null)
				return;

			item.OnIsAvaliableChanged += ItemOnIsAvaliableChanged;
		}

		private void Unsubscibe(IFloatingActionListItemPresenter item)
		{
			if (item == null)
				return;

			item.OnIsAvaliableChanged -= ItemOnIsAvaliableChanged;
		}

		private void ItemOnIsAvaliableChanged(object sender, BoolEventArgs e)
		{
			bool refreshNeeded = UpdateVisibility();

			if (refreshNeeded)
				Refresh();
		}

		#endregion
	}
}
