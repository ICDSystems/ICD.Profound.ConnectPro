using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	[ViewBinding(typeof(IMenuCombinedAdvancedModeView))]
	public sealed partial class MenuCombinedAdvancedModeView : AbstractUiView, IMenuCombinedAdvancedModeView
	{
		public event EventHandler OnSimpleModeButtonPressed;
		public event EventHandler OnRouteSummaryButtonPressed;

		private readonly List<IReferencedAdvancedDisplayView> m_ChildViews;

		public MenuCombinedAdvancedModeView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
			m_ChildViews = new List<IReferencedAdvancedDisplayView>();
		}

		public void SetSimpleModeButtonEnabled(bool enabled)
		{
			m_SimpleModeButton.Enable(enabled);
		}

		public void SetRouteSummaryButtonEnabled(bool enabled)
		{
			m_RouteSummaryButton.Enable(enabled);
		}

		public IEnumerable<IReferencedAdvancedDisplayView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return factory.LazyLoadSrlViews(m_DisplaysList, m_ChildViews, count);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_SimpleModeButton.OnPressed += SimpleModeButtonOnPressed;
			m_RouteSummaryButton.OnPressed += RouteSummaryButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_SimpleModeButton.OnPressed -= SimpleModeButtonOnPressed;
			m_RouteSummaryButton.OnPressed -= RouteSummaryButtonOnPressed;
		}

		private void SimpleModeButtonOnPressed(object sender, EventArgs e)
		{
			OnSimpleModeButtonPressed.Raise(this);
		}

		private void RouteSummaryButtonOnPressed(object sender, EventArgs e)
		{
			OnRouteSummaryButtonPressed.Raise(this);
		}

		#endregion
	}
}
