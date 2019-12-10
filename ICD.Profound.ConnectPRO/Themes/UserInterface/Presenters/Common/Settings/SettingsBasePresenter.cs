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
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	[PresenterBinding(typeof(ISettingsBasePresenter))]
	public sealed class SettingsBasePresenter : AbstractPopupPresenter<ISettingsBaseView>, ISettingsBasePresenter
	{
		private readonly List<ISettingsNodeBase> m_MenuPath;
		private readonly SafeCriticalSection m_RefreshSection;

		[CanBeNull]
		private RootSettingsNode m_SettingsRoot;

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
		public SettingsBasePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
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

				m_SettingsRoot = room == null ? null : new RootSettingsNode(room);

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
				// Show the back button
				bool backButton = m_MenuPath.Count >= 2;

				// Edge case - don't show back button for leaves on the root
				if (CurrentNode is ISettingsLeaf && ParentNode == m_SettingsRoot)
					backButton = false;

				view.SetBackButtonVisible(backButton);

				// Set the title
				SetTitle(view);

				// Populate the buttons
				ISettingsNodeBase[] children = new ISettingsNodeBase[0];

				if (CurrentNode is ISettingsLeaf && ParentNode != null)
					children = ParentNode.GetChildren().ToArray();
				else if (CurrentNode is ISettingsNode)
					children = (CurrentNode as ISettingsNode).GetChildren().ToArray();

				IEnumerable<KeyValuePair<string, string>> namesAndIcons = children.Select(c => GetNameAndIcon(c));

				view.SetButtonLabels(namesAndIcons);
				for (ushort index = 0; index < children.Length; index++)
				{
					view.SetButtonVisible(index, children[index].Visible);
					view.SetButtonSelected(index, children[index] == CurrentNode);
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
			string icon = SettingsTreeIcons.GetIcon(node.Icon, SettingsTreeIcons.eColor.Gray);

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
				while (m_MenuPath.LastOrDefault() is ISettingsLeaf)
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

		private void SetTitle(ISettingsBaseView view)
		{
			if (m_MenuPath.Count == 0)
			{
				view.SetTitle("Settings", "");
				return;
			}

			//If current node isn't a leaf, show it as parent, no leaf
			if (CurrentNode is ISettingsLeaf)
			{
				if (ParentNode != null)
					view.SetTitle(ParentNode.Name, CurrentNode.Name);
			}
			else if (CurrentNode != null)
				view.SetTitle(CurrentNode.Name, "");
			else
				view.SetTitle("Settings", "");
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

			view.OnListItemPressed += ViewOnListItemPressed;
			view.OnBackButtonPressed += ViewOnBackButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsBaseView view)
		{
			base.Unsubscribe(view);

			view.OnListItemPressed -= ViewOnListItemPressed;
			view.OnBackButtonPressed -= ViewOnBackButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the back button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnBackButtonPressed(object sender, EventArgs eventArgs)
		{
			Back();
		}

		/// <summary>
		/// Called when the user presses a list item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnListItemPressed(object sender, UShortEventArgs eventArgs)
		{
			m_RefreshSection.Enter();

			try
			{
				ISettingsNode node = CurrentNode as ISettingsNode ?? ParentNode;
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
