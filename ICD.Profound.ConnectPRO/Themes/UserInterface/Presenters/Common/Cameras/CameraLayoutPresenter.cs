using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Layout;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Components.Layout;
using ICD.Connect.Conferencing.Zoom.Controls;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Cameras
{
	[PresenterBinding(typeof(ICameraLayoutPresenter))]
	public sealed class CameraLayoutPresenter : AbstractUiPresenter<ICameraLayoutView>, ICameraLayoutPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		[CanBeNull] private IConferenceLayoutControl m_ConferenceLayoutControl;
		[CanBeNull] private LayoutComponent m_SubscribedZoomLayoutComponent;

		private bool m_ZoomMode;
		private ushort m_SizeIndex;
		private ushort m_StyleIndex;
		private ushort m_ShareThumbIndex;
		private ushort m_SelfViewIndex;
		private ushort m_PositionIndex;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CameraLayoutPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) 
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_ConferenceLayoutControl);
			Unsubscribe(m_SubscribedZoomLayoutComponent);
		}

		public void SetDestinationControl(IConferenceLayoutControl value)
		{
			if (value == m_ConferenceLayoutControl)
				return;

			m_ConferenceLayoutControl = value;
			Subscribe(m_ConferenceLayoutControl);
			if (m_ConferenceLayoutControl is ZoomRoomLayoutControl)
			{
				m_ZoomMode = true;
				var control = m_ConferenceLayoutControl as ZoomRoomLayoutControl;
				m_SubscribedZoomLayoutComponent = control
				                                  .Parent.Components
				                                  .GetComponent<LayoutComponent>();

				Subscribe(m_SubscribedZoomLayoutComponent);
			}
		}

		protected override void Refresh(ICameraLayoutView view)
		{
			base.Refresh(view);

			if (m_ConferenceLayoutControl != null && m_ConferenceLayoutControl.LayoutAvailable && m_ZoomMode)
			{
				view.SetLayoutSizeListEnabled(true);
				view.SetLayoutStyleListEnable(true);
				view.SetLayoutShareListEnable(true);
				view.SetLayoutSelfViewListEnable(true);
				view.SetLayoutPositionListEnable(true);
			}

			//Set size selected
			for (ushort i = 0; i <= 4; i++)
				view.SetLayoutSizeControlButtonSelected(i, i == m_SizeIndex);
			//Set style selected
			for (ushort i = 0; i <= 3; i++)
				view.SetLayoutStyleControlButtonSelected(i, i == m_StyleIndex);
			//Set share selected
			for (ushort i = 0; i <= 1; i++)
				view.SetLayoutShareControlButtonSelected(i, i == m_ShareThumbIndex);
			//Set self-view selected
			for (ushort i = 0; i <= 1; i++)
				view.SetLayoutSelfViewControlButtonSelected(i, i == m_SelfViewIndex);
			//Set position selected
			for (ushort i = 0; i <= 3; i++)
				view.SetLayoutPositionControlButtonSelected(i, i == m_PositionIndex);
		}

		#region Control Callbacks

		private void Subscribe(IConferenceLayoutControl control)
		{
			if (control == null)
				return;

			control.OnLayoutAvailableChanged += ControlOnLayoutAvailableChanged;
			control.OnSelfViewEnabledChanged += ControlOnSelfViewEnabledChanged;
			control.OnSelfViewFullScreenEnabledChanged += ControlOnSelfViewFullScreenEnabledChanged;
		}

		private void Unsubscribe(IConferenceLayoutControl control)
		{
			if (control == null)
				return;

			control.OnLayoutAvailableChanged -= ControlOnLayoutAvailableChanged;
			control.OnSelfViewEnabledChanged -= ControlOnSelfViewEnabledChanged;
			control.OnSelfViewFullScreenEnabledChanged -= ControlOnSelfViewFullScreenEnabledChanged;
		}

		private void ControlOnSelfViewFullScreenEnabledChanged(object sender, ConferenceLayoutSelfViewFullScreenApiEventArgs e)
		{
			RefreshIfVisible();
		}

		private void ControlOnSelfViewEnabledChanged(object sender, ConferenceLayoutSelfViewApiEventArgs e)
		{
			m_SelfViewIndex = e.Data ? (ushort)1 : (ushort)0;

			RefreshIfVisible();
		}

		private void ControlOnLayoutAvailableChanged(object sender, ConferenceLayoutAvailableApiEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region ZoomRoom Component Callbacks

		private void Subscribe(LayoutComponent layoutComponent)
		{
			if (layoutComponent == null)
				return;

			layoutComponent.OnPositionChanged += LayoutComponentOnPositionChanged;
			layoutComponent.OnShareThumbChanged += LayoutComponentOnShareThumbChanged;
			layoutComponent.OnSizeChanged += LayoutComponentOnSizeChanged;
			layoutComponent.OnStyleChanged += LayoutComponentOnStyleChanged;
		}

		private void Unsubscribe(LayoutComponent layoutComponent)
		{
			if (layoutComponent == null)
				return;

			layoutComponent.OnPositionChanged -= LayoutComponentOnPositionChanged;
			layoutComponent.OnShareThumbChanged -= LayoutComponentOnShareThumbChanged;
			layoutComponent.OnSizeChanged -= LayoutComponentOnSizeChanged;
			layoutComponent.OnStyleChanged -= LayoutComponentOnStyleChanged;
		}

		private void LayoutComponentOnStyleChanged(object sender, ZoomLayoutStyleEventArgs e)
		{
			switch (e.LayoutStyle)
			{
				case eZoomLayoutStyle.Gallery:
					m_StyleIndex = 0;
					break;
				case eZoomLayoutStyle.Speaker:
					m_StyleIndex = 1;
					break;
				case eZoomLayoutStyle.Strip:
					m_StyleIndex = 2;
					break;
				case eZoomLayoutStyle.ShareAll:
					m_StyleIndex = 3;
					break;
				case eZoomLayoutStyle.None:
					m_StyleIndex = 4;
					break;
				default:
					throw new ArgumentOutOfRangeException("e");
			}

			RefreshIfVisible();
		}

		private void LayoutComponentOnSizeChanged(object sender, ZoomLayoutSizeEventArgs e)
		{
			switch (e.LayoutSize)
			{
				case eZoomLayoutSize.Size1:
					m_SizeIndex = 1;
					break;
				case eZoomLayoutSize.Size2:
					m_SizeIndex = 2;
					break;
				case eZoomLayoutSize.Size3:
					m_SizeIndex = 3;
					break;
				case eZoomLayoutSize.Strip:
					m_SizeIndex = 4;
					break;
				case eZoomLayoutSize.Off:
					m_SizeIndex = 0;
					break;
				case eZoomLayoutSize.None:
					m_SizeIndex = 5;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			RefreshIfVisible();
		}

		private void LayoutComponentOnShareThumbChanged(object sender, BoolEventArgs e)
		{
			m_ShareThumbIndex = e.Data ? (ushort)1 : (ushort)0;

			RefreshIfVisible();
		}

		private void LayoutComponentOnPositionChanged(object sender, ZoomLayoutPositionEventArgs e)
		{
			switch (e.LayoutPosition)
			{
				case eZoomLayoutPosition.UpRight:
					m_PositionIndex = 1;
					break;
				case eZoomLayoutPosition.DownRight:
					m_PositionIndex = 3;
					break;
				case eZoomLayoutPosition.UpLeft:
					m_PositionIndex = 0;
					break;
				case eZoomLayoutPosition.DownLeft:
					m_PositionIndex = 2;
					break;
				case eZoomLayoutPosition.None:
				case eZoomLayoutPosition.Center:
				case eZoomLayoutPosition.Up:
				case eZoomLayoutPosition.Right:
				case eZoomLayoutPosition.Down:
				case eZoomLayoutPosition.Left:
					m_PositionIndex = 4;
					break;

				default:
					throw new ArgumentOutOfRangeException("e");
			}

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(ICameraLayoutView view)
		{
			base.Subscribe(view);
			
			view.OnLayoutSizeButtonPressed += ViewOnLayoutSizeButtonPressed;
			view.OnLayoutStyleButtonPressed += ViewOnLayoutStyleButtonPressed;
			view.OnLayoutShareButtonPressed += ViewOnLayoutShareButtonPressed;
			view.OnLayoutSelfViewButtonPressed += ViewOnLayoutSelfViewButtonPressed;
			view.OnLayoutPositionButtonPressed += ViewOnLayoutPositionButtonPressed;
		}

		protected override void Unsubscribe(ICameraLayoutView view)
		{
			base.Unsubscribe(view);
			
			view.OnLayoutSizeButtonPressed -= ViewOnLayoutSizeButtonPressed;
			view.OnLayoutStyleButtonPressed -= ViewOnLayoutStyleButtonPressed;
			view.OnLayoutShareButtonPressed -= ViewOnLayoutShareButtonPressed;
			view.OnLayoutSelfViewButtonPressed -= ViewOnLayoutSelfViewButtonPressed;
			view.OnLayoutPositionButtonPressed -= ViewOnLayoutPositionButtonPressed;
		}

		private void ViewOnLayoutSizeButtonPressed(object sender, UShortEventArgs e)
		{
			m_RefreshSection.Enter();

			try
			{
				var control = m_ConferenceLayoutControl as ZoomRoomLayoutControl;
				if (control == null)
					return;

				eZoomLayoutSize size;
				switch (e.Data)
				{
					case 0:
						size = eZoomLayoutSize.Off;
						break;
					case 1:
						size = eZoomLayoutSize.Size1;
						break;
					case 2:
						size = eZoomLayoutSize.Size2;
						break;
					case 3:
						size = eZoomLayoutSize.Size3;
						break;
					case 4:
						size = eZoomLayoutSize.Strip;
						break;

					default:
						size = eZoomLayoutSize.None;
						break;
				}

				control.SetLayoutSize(size);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void ViewOnLayoutStyleButtonPressed(object sender, UShortEventArgs e)
		{
			m_RefreshSection.Enter();

			try
			{
				var control = m_ConferenceLayoutControl as ZoomRoomLayoutControl;
				if (control == null)
					return;

				eZoomLayoutStyle style;
				switch (e.Data)
				{
					case 0:
						style = eZoomLayoutStyle.Gallery;
						break;
					case 1:
						style = eZoomLayoutStyle.Speaker;
						break;
					case 2:
						style = eZoomLayoutStyle.Strip;
						break;
					case 3:
						style = eZoomLayoutStyle.ShareAll;
						break;

					default:
						style = eZoomLayoutStyle.None;
						break;
				}

				control.SetStyle(style);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void ViewOnLayoutShareButtonPressed(object sender, UShortEventArgs e)
		{
			m_RefreshSection.Enter();

			try
			{
				var control = m_ConferenceLayoutControl as ZoomRoomLayoutControl;
				if (control == null)
					return;

				bool enabled = e.Data == 1;
				control.SetShareThumb(enabled);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void ViewOnLayoutSelfViewButtonPressed(object sender, UShortEventArgs e)
		{
			m_RefreshSection.Enter();

			try
			{
				bool enabled = e.Data == 1;
				if (m_ConferenceLayoutControl != null)
					m_ConferenceLayoutControl.SetSelfViewEnabled(enabled);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void ViewOnLayoutPositionButtonPressed(object sender, UShortEventArgs e)
		{
			m_RefreshSection.Enter();

			try
			{
				var control = m_ConferenceLayoutControl as ZoomRoomLayoutControl;
				if (control == null)
					return;

				eZoomLayoutPosition position;
				switch (e.Data)
				{
					case 0:
						position = eZoomLayoutPosition.UpLeft;
						break;
					case 1:
						position = eZoomLayoutPosition.UpRight;
						break;
					case 2:
						position = eZoomLayoutPosition.DownLeft;
						break;
					case 3:
						position = eZoomLayoutPosition.DownRight;
						break;

					default:
						position = eZoomLayoutPosition.None;
						break;
				}

				control.SetLayoutPosition(position);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion
	}
}
