using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.ActiveCalls
{
	public sealed partial class VtcActiveCallsView : AbstractUiView, IVtcActiveCallsView
	{
		public event EventHandler OnHangupAllButtonPressed;
		public event EventHandler OnCloseButtonPressed;

		private readonly List<IVtcReferencedActiveCallsView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcActiveCallsView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IVtcReferencedActiveCallsView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnHangupAllButtonPressed = null;
			OnCloseButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IVtcReferencedActiveCallsView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_HangupList, m_ChildList, count);
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_HangupAllButton.OnPressed += HangupAllButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_HangupAllButton.OnPressed -= HangupAllButtonOnPressed;
		}

		private void HangupAllButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnHangupAllButtonPressed.Raise(this);
		}
	}
}
