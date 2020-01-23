using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.SettingsTree;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Settings;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Settings
{
	[PresenterBinding(typeof(ISettingsBasePresenter))]
	public sealed class SettingsBasePresenter : AbstractPopupPresenter<ISettingsBaseView>, ISettingsBasePresenter
	{
		private readonly List<ISettingsNodeBase> m_MenuPath;
		private readonly SafeCriticalSection m_RefreshSection;

		[CanBeNull]
		private IRootSettingsNode m_SettingsRoot;

		private ISettingsNodeBasePresenter m_CurrentPresenter;

		/// <summary>
		/// Node to show upon successfuly password entry
		/// </summary>
		private ISettingsNodeBase m_PasscodeSuccessNode;

		#region Properties

		/// <summary>
		/// Gets the current settings node item.
		/// </summary>
		[CanBeNull]
		private ISettingsNodeBase CurrentNode
		{
			get
			{
				return m_MenuPath.Count == 0
					       ? null
					       : m_MenuPath[m_MenuPath.Count - 1];
			}
		}

		/// <summary>
		/// Gets the parent settings node item.
		/// </summary>
		[CanBeNull]
		private ISettingsNode ParentNode
		{
			get
			{
				return m_MenuPath.Count <= 1
					       ? null
					       : m_MenuPath[m_MenuPath.Count - 2] as ISettingsNode;
			}
		}

		/// <summary>
		/// Gets the parent settings node item.
		/// </summary>
		[CanBeNull]
		private ISettingsNode ParentParentNode
		{
			get
			{
				return m_MenuPath.Count <= 2
					? null
					: m_MenuPath[m_MenuPath.Count - 3] as ISettingsNode;
			}
		}

		/// <summary>
		/// Tracks if the user has logged into the settings pages
		/// </summary>
		public bool IsLoggedIn { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsBasePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_MenuPath = new List<ISettingsNodeBase>();
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			if (m_SettingsRoot != null)
				m_SettingsRoot.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			m_RefreshSection.Enter();

			try
			{
				if (m_SettingsRoot != null)
					m_SettingsRoot.Dispose();

				m_SettingsRoot = room == null ? null : new TouchDisplayRootSettingsNode(room);

				m_MenuPath.Clear();
				HideCurrentPresenter();

				if (m_SettingsRoot != null)
					NavigateTo(m_SettingsRoot);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsBaseView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				if (CurrentNode == null)
					return;

				ISettingsNode primaryNode = ParentNode != null
					? ParentParentNode != null || CurrentNode is ISettingsLeaf 
						? ParentParentNode
						: ParentNode
					: CurrentNode as ISettingsNode;

				ISettingsNode secondaryNode = ParentNode != null 
					? ParentParentNode != null && CurrentNode is ISettingsLeaf 
						? ParentNode 
						: CurrentNode as ISettingsNode 
					: null;
				
				view.SetSecondaryButtonsVisibility(secondaryNode != null);
				if (secondaryNode != null)
				{
					ISettingsNodeBase[] secondaryChildren = secondaryNode.GetChildren().ToArray();
					IEnumerable<KeyValuePair<string, string>> secondaryNamesAndIcons =
						secondaryChildren.Select(c => GetNameAndIcon(c));
					view.SetSecondaryButtonLabels(secondaryNamesAndIcons);
					for (ushort index = 0; index < secondaryChildren.Length; index++)
					{
						view.SetSecondaryButtonVisible(index, secondaryChildren[index].Visible);
						view.SetSecondaryButtonSelected(index, IsNodeSelected(secondaryChildren[index]));
					}
				}

				ISettingsNodeBase[] primaryChildren = primaryNode.GetChildren().ToArray();
				IEnumerable<KeyValuePair<string, string>> primaryNamesAndIcons =
					primaryChildren.Select(c => GetNameAndIcon(c));
				view.SetPrimaryButtonLabels(primaryNamesAndIcons);
				for (ushort index = 0; index < primaryChildren.Length; index++)
				{
					view.SetPrimaryButtonVisible(index, primaryChildren[index].Visible);
					view.SetPrimaryButtonSelected(index, IsNodeSelected(primaryChildren[index]));
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		private static KeyValuePair<string, string> GetNameAndIcon(ISettingsNodeBase node)
		{
			string name = node.Name;
			string icon = TouchCueIcons.GetIcon(node.Icon);

			return new KeyValuePair<string, string>(name, icon);
		}

		private void HideCurrentPresenter()
		{
			m_RefreshSection.Enter();

			try
			{
				if (m_CurrentPresenter != null)
				{
					m_CurrentPresenter.ShowView(false);
					m_CurrentPresenter.Node = null;
				}

				m_CurrentPresenter = null;
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private bool IsNodeSelected(ISettingsNodeBase settingsNode)
		{
			return m_MenuPath.Contains(settingsNode);
		}

		private void ShowPresenterForCurrentNode()
		{
			m_RefreshSection.Enter();

			try
			{
				HideCurrentPresenter();

				if (CurrentNode == null)
					return;

				m_CurrentPresenter = GetPresenterForNode(CurrentNode);

				m_CurrentPresenter.Node = CurrentNode;
				m_CurrentPresenter.ShowView(IsViewVisible);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void NavigateTo(ISettingsNodeBase node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			m_RefreshSection.Enter();

			try
			{
				// Prevents flicker when selecting the current node
				if (node == CurrentNode)
					return;
				
				// Check if node requires login, and if not logged in, prompt for pin
				if (node.RequiresLogin && !IsLoggedIn)
				{
					m_PasscodeSuccessNode = node;
					Navigation.LazyLoadPresenter<ISettingsPasscodePresenter>().ShowView(PasscodeSuccessCallback);
					return;
				}

				// Remove any leaves from the list
				while (CurrentNode is ISettingsLeaf)
					m_MenuPath.RemoveAt(m_MenuPath.Count - 1);

				// Remove nodes until we find the node we want
				while (CurrentNode is ISettingsNode && !(CurrentNode as ISettingsNode).GetChildren().Contains(node))
					m_MenuPath.RemoveAt(m_MenuPath.Count - 1);

				// Append the new node to the list
				m_MenuPath.Add(node);

				// Keep navigating while node only has one child
				ISettingsNode parent = node as ISettingsNode;
				ISettingsNodeBase[] children = parent == null ? null : parent.GetChildren().Where(c => c.Visible).ToArray();

				if (children != null && children.Length == 1)
				{
					NavigateTo(children[0]);
					return;
				}

				ShowPresenterForCurrentNode();
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		private void Back()
		{
			m_RefreshSection.Enter();

			try
			{
				// Don't back out of the root
				if (m_MenuPath.Count <= 1)
					return;

				// We want to back out of the current leaf AND the parent node
				if (CurrentNode is ISettingsLeaf)
				{
					m_MenuPath.RemoveAt(m_MenuPath.Count - 1);
					Back();
					return;
				}

				m_MenuPath.RemoveAt(m_MenuPath.Count - 1);

				// Keep going back if there is only one child
				ISettingsNode newParent = CurrentNode as ISettingsNode;
				if (newParent != null && m_MenuPath.Count > 1 && newParent.GetChildren().Count(c => c.Visible) <= 1)
				{
					Back();
					return;
				}

				ShowPresenterForCurrentNode();
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		private ISettingsNodeBasePresenter GetPresenterForNode(ISettingsNodeBase node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			Type type = node.GetType();

			// Lazy - Optimize?
			return Navigation.LazyLoadPresenters<ISettingsNodeBasePresenter>().First(p => type.IsAssignableTo(p.NodeType));
		}

		private void SaveDirtySettings()
		{
			if (m_SettingsRoot == null || !m_SettingsRoot.Dirty)
				return;

			Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>().ShowView("Saving Settings", 60 * 1000);
			m_SettingsRoot.SaveDirtySettings();
			Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>().ShowView(false);
		}

		/// <summary>
		/// Called when the user successfully enters the passcode.
		/// </summary>
		/// <param name="sender"></param>
		private void PasscodeSuccessCallback(ISettingsPasscodePresenter sender)
		{
			IsLoggedIn = true;

			Navigation.LazyLoadPresenter<ISettingsPasscodePresenter>().ShowView(false);

			if (m_PasscodeSuccessNode != null)
				NavigateTo(m_PasscodeSuccessNode);
			m_PasscodeSuccessNode = null;
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsBaseView view)
		{
			base.Subscribe(view);

			view.OnPrimaryListItemPressed += ViewOnPrimaryListItemPressed;
			view.OnSecondaryListItemPressed += ViewOnSecondaryListItemPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsBaseView view)
		{
			base.Unsubscribe(view);

			view.OnPrimaryListItemPressed -= ViewOnPrimaryListItemPressed;
			view.OnSecondaryListItemPressed -= ViewOnSecondaryListItemPressed;
		}

		/// <summary>
		/// Called when the user presses a list item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPrimaryListItemPressed(object sender, UShortEventArgs eventArgs)
		{
			m_RefreshSection.Enter();

			try
			{
				ISettingsNode node = ParentNode != null
					? ParentParentNode != null || CurrentNode is ISettingsLeaf 
						? ParentParentNode
						: ParentNode
					: CurrentNode as ISettingsNode;
				if (node == null)
					return;

				ISettingsNodeBase child;
				if (!node.GetChildren().TryElementAt(eventArgs.Data, out child))
					return;

				NavigateTo(child);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the user presses a list item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSecondaryListItemPressed(object sender, UShortEventArgs eventArgs)
		{
			m_RefreshSection.Enter();

			try
			{
				ISettingsNode node = ParentNode != null 
					? ParentParentNode != null && CurrentNode is ISettingsLeaf 
						? ParentNode 
						: CurrentNode as ISettingsNode 
					: null;
				if (node == null)
					return;

				ISettingsNodeBase child;
				if (!node.GetChildren().TryElementAt(eventArgs.Data, out child))
					return;

				NavigateTo(child);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the view visibility is about to change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			if (!args.Data)
				HideCurrentPresenter();

			// Return to the root but don't show it
			if (m_MenuPath.Count > 1)
				m_MenuPath.RemoveRange(1, m_MenuPath.Count - 1);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (args.Data)
				ShowPresenterForCurrentNode();
			else
			{
				IsLoggedIn = false;
				SaveDirtySettings();
			}
		}

		#endregion
	}
}
