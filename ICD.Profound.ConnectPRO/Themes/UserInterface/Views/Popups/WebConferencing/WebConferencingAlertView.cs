using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.WebConferencing
{
	public sealed partial class WebConferencingAlertView : AbstractUiView, IWebConferencingAlertView
	{
		public event EventHandler OnDismissButtonPressed;

		private readonly List<IReferencedWebConferencingAlertView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public WebConferencingAlertView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedWebConferencingAlertView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDismissButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedWebConferencingAlertView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_AppList, m_ChildList, count);
		}

		public void SetAppCount(ushort count)
		{
			m_AppList.SetNumberOfItems(count);
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DismissButton.OnPressed += DismissButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DismissButton.OnPressed -= DismissButtonOnPressed;
		}

		private void DismissButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDismissButtonPressed.Raise(this);
		}
	}
}
