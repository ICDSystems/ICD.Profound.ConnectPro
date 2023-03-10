using System;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.VisibilityTree;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.FooterNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.VisibilityTree
{
	/// <summary>
	/// Handles the notification tray visibility of the Profound CUE.
	/// Combines SingleVisibilityNode and DefaultVisibilityNode functionality,
	/// while making sure the HelloPresenter is always visible if not in the notification area.
	/// </summary>
	public sealed class NotificationVisibilityNode : AbstractVisibilityNode
	{
		private readonly IOsdHelloFooterNotificationPresenter m_HelloPresenter;

		public NotificationVisibilityNode(IOsdHelloFooterNotificationPresenter helloPresenter)
		{
			if (helloPresenter == null)
				throw new ArgumentNullException("helloPresenter");

			m_HelloPresenter = helloPresenter;
			
			m_HelloPresenter.OnMainPageViewChanged += HelloPresenterOnMainPageViewChanged;
		}

		private void HelloPresenterOnMainPageViewChanged(object sender, BoolEventArgs args)
		{
			// if main view activates, show the HelloPresenter.
			if (args.Data)
				m_HelloPresenter.ShowView(true);

			// if main view deactivates, hide the HelloPresenter if a notification is showing
			if (!args.Data && IsVisible)
				m_HelloPresenter.ShowView(false);
		}

		/// <summary>
		/// Called when a child presenter visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void PresenterOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.PresenterOnVisibilityChanged(sender, args);
			
			if (!args.Data && !IsVisible)
				m_HelloPresenter.ShowView(true);

			if (args.Data)
			{
				HideExcept(sender as IOsdPresenter);
				HideExcept(null as IVisibilityNode);
			}
		}

		/// <summary>
		/// Called when a descendant presenter changes visibility.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="presenter"></param>
		/// <param name="visibility"></param>
		protected override void NodeOnChildVisibilityChanged(IVisibilityNode parent, IPresenter presenter, bool visibility)
		{
			base.NodeOnChildVisibilityChanged(parent, presenter, visibility);

			if (!visibility && !IsVisible)
				m_HelloPresenter.ShowView(true);

			if (visibility)
			{
				HideExcept(null as IOsdPresenter);
				HideExcept(parent);
			}
		}

		/// <summary>
		/// Hides child presenters except the given presenter.
		/// </summary>
		/// <param name="ignoreControl"></param>
		private void HideExcept(IOsdPresenter ignoreControl)
		{
			foreach (IPresenter presenter in GetPresenters().Where(c => c != ignoreControl))
				presenter.ShowView(false);

			// only hide the HelloPresenter if Main Page View is not activated
			if (!m_HelloPresenter.MainPageView && ignoreControl != m_HelloPresenter)
				m_HelloPresenter.ShowView(false);
		}

		/// <summary>
		/// Hides child nodes except the given node.
		/// </summary>
		/// <param name="ignoreNode"></param>
		private void HideExcept(IVisibilityNode ignoreNode)
		{
			foreach (IVisibilityNode node in GetNodes().Where(n => n != ignoreNode))
				node.Hide();
		}
	}
}
