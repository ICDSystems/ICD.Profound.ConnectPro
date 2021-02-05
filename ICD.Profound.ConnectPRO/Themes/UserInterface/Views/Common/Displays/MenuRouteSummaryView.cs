
using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	[ViewBinding(typeof(IMenuRouteSummaryView))]
	public sealed partial class MenuRouteSummaryView : AbstractUiView, IMenuRouteSummaryView
	{
		public event EventHandler OnCloseButtonPressed;

		private readonly List<IReferencedRouteListItemView> m_ChildViews;

		public MenuRouteSummaryView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildViews = new List<IReferencedRouteListItemView>();
		}

		public IEnumerable<IReferencedRouteListItemView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return factory.LazyLoadSrlViews(m_RouteList, m_ChildViews, count);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
		}

		private void CloseButtonOnPressed(object sender, EventArgs e)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}
