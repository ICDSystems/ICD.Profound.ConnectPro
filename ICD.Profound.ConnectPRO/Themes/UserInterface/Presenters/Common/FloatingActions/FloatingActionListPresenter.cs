using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions.FloatingActionListItems;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions
{
	[PresenterBinding(typeof(IFloatingActionListPresenter))]
	public sealed class FloatingActionListPresenter : AbstractUiPresenter<IFloatingActionListView> , IFloatingActionListPresenter
	{

		private readonly List<IFloatingActionListItem> m_ListItems;

		private readonly SafeCriticalSection m_RefreshSection;

		private readonly Dictionary<int, IFloatingActionListItem> m_AvaliableListItems;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public FloatingActionListPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ListItems = new List<IFloatingActionListItem>();
			m_AvaliableListItems = new Dictionary<int, IFloatingActionListItem>();
		}

		public void SetChildren(IEnumerable<IFloatingActionListItem> items)
		{
			m_RefreshSection.Enter();

			try
			{
				m_ListItems.Clear();
				m_ListItems.AddRange(items);

				RefreshIfVisible();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IFloatingActionListView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				//Show avaliable items in order, and add them to dictionary for lookup in OnPressed
				int avaliableIndex = 0;
				m_AvaliableListItems.Clear();
				foreach (IFloatingActionListItem item in m_ListItems)
				{
					if (!item.IsAvailable)
						continue;
					m_AvaliableListItems.Add(avaliableIndex, item);
					view.ShowItem(avaliableIndex, item.Label, Icons.GetIcon(item.Icon, eIconColor.White));
					avaliableIndex++;
				}

				//Clear unused items
				for (int i = avaliableIndex; i < view.MaxItems; i++)
				{
					view.HideItem(i);
				}

			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IFloatingActionListView view)
		{
			base.Subscribe(view);

			if (view == null)
				return;

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IFloatingActionListView view)
		{
			base.Unsubscribe(view);

			if (view == null)
				return;

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		private void ViewOnButtonPressed(object sender, IntEventArgs args)
		{
			IFloatingActionListItem item;
			if (m_AvaliableListItems.TryGetValue(args.Data, out item))
				item.HandleButtonPressed();
		}
	}
}
