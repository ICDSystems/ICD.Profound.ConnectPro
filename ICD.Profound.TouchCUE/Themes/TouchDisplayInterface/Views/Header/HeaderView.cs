using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Header
{
	[ViewBinding(typeof(IHeaderView))]
	public sealed partial class HeaderView : AbstractTouchDisplayView, IHeaderView
	{
		public event EventHandler OnCenterButtonPressed;
		public event EventHandler OnCollapseButtonPressed;

		private readonly List<IReferencedHeaderButtonView> m_LeftButtonViewList;
		private readonly List<IReferencedHeaderButtonView> m_RightButtonViewList;

		public HeaderView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
			m_LeftButtonViewList = new List<IReferencedHeaderButtonView>();
			m_RightButtonViewList = new List<IReferencedHeaderButtonView>();
		}

		public void SetRoomName(string name)
		{
			m_RoomName.SetLabelText(name);
		}

		public void SetTimeLabel(string time)
		{
			m_TimeLabel.SetLabelText(time);
		}

		public void SetCenterButtonMode(eCenterButtonMode mode)
		{
			m_CenterButton.SetMode((ushort)mode);
		}

		public void SetCenterButtonSelected(bool selected)
		{
			m_CenterButton.SetSelected(selected);
		}

		public void SetCenterButtonEnabled(bool enabled)
		{
			m_CenterButton.Enable(enabled);
		}

		public void SetCenterButtonText(string text)
		{
			m_CenterButton.SetLabelText(text);
		}

		public void SetCollapsed(bool collapsed)
		{
			m_CollapseButton.SetSelected(collapsed);
		}

		public IEnumerable<IReferencedHeaderButtonView> GetLeftButtonViews(ITouchDisplayViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_LeftButtonList, m_LeftButtonViewList, count);
		}

		public IEnumerable<IReferencedHeaderButtonView> GetRightButtonViews(ITouchDisplayViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_RightButtonList, m_RightButtonViewList, count);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CenterButton.OnPressed += StartEndMeetingButtonOnPressed;
			m_CollapseButton.OnPressed += CollapseButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CenterButton.OnPressed -= StartEndMeetingButtonOnPressed;
			m_CollapseButton.OnPressed -= CollapseButtonOnOnPressed;
		}

		private void StartEndMeetingButtonOnPressed(object sender, EventArgs e)
		{
			OnCenterButtonPressed.Raise(this);
		}

		private void CollapseButtonOnOnPressed(object sender, EventArgs e)
		{
			OnCollapseButtonPressed.Raise(this);
		}

		#endregion
	}
}