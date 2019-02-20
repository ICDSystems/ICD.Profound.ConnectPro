using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.EventArguments;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.CableTv
{
	[ViewBinding(typeof(ICableTvView))]
	public sealed partial class CableTvView : AbstractPopupView, ICableTvView
	{
		public override event EventHandler OnCloseButtonPressed;

		public event EventHandler OnGuideButtonPressed;
		public event EventHandler OnExitButtonPressed;
		public event EventHandler OnPowerButtonPressed;

		public event EventHandler<CharEventArgs> OnNumberButtonPressed;
		public event EventHandler OnClearButtonPressed;
		public event EventHandler OnEnterButtonPressed;

		public event EventHandler OnUpButtonPressed;
		public event EventHandler OnDownButtonPressed;
		public event EventHandler OnLeftButtonPressed;
		public event EventHandler OnRightButtonPressed;
		public event EventHandler OnSelectButtonPressed;

		public event EventHandler OnChannelUpButtonPressed;
		public event EventHandler OnChannelDownButtonPressed;
		public event EventHandler OnPageUpButtonPressed;
		public event EventHandler OnPageDownButtonPressed;

		private readonly List<IReferencedCableTvView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public CableTvView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedCableTvView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCloseButtonPressed = null;

			OnGuideButtonPressed = null;
			OnExitButtonPressed = null;
			OnPowerButtonPressed = null;

			OnNumberButtonPressed = null;
			OnClearButtonPressed = null;
			OnEnterButtonPressed = null;

			OnUpButtonPressed = null;
			OnDownButtonPressed = null;
			OnLeftButtonPressed = null;
			OnRightButtonPressed = null;
			OnSelectButtonPressed = null;

			OnChannelUpButtonPressed = null;
			OnChannelDownButtonPressed = null;
			OnPageUpButtonPressed = null;
			OnPageDownButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedCableTvView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ChannelList, m_ChildList, count);
		}

		public void ShowSwipeIcons(bool show)
		{
			m_SwipeLeftButton.Show(show);
			m_SwipeRightButton.Show(show);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
			m_GuideButton.OnPressed += GuideButtonOnPressed;
			m_ExitButton.OnPressed += ExitButtonOnPressed;
			m_PowerButton.OnPressed += PowerButtonOnPressed;
			m_MenuDirections.OnButtonPressed += MenuDirectionsOnButtonPressed;
			m_NumberKeypad.OnButtonPressed += NumberKeypadOnButtonPressed;
			m_ChannelUpButton.OnPressed += ChannelUpButtonOnPressed;
			m_ChannelDownButton.OnPressed += ChannelDownButtonOnPressed;
			m_PageUpButton.OnPressed += PageUpButtonOnPressed;
			m_PageDownButton.OnPressed += PageDownButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
			m_GuideButton.OnPressed -= GuideButtonOnPressed;
			m_ExitButton.OnPressed -= ExitButtonOnPressed;
			m_PowerButton.OnPressed -= PowerButtonOnPressed;
			m_MenuDirections.OnButtonPressed -= MenuDirectionsOnButtonPressed;
			m_NumberKeypad.OnButtonPressed -= NumberKeypadOnButtonPressed;
			m_ChannelUpButton.OnPressed -= ChannelUpButtonOnPressed;
			m_ChannelDownButton.OnPressed -= ChannelDownButtonOnPressed;
			m_PageUpButton.OnPressed -= PageUpButtonOnPressed;
			m_PageDownButton.OnPressed -= PageDownButtonOnPressed;
		}

		private void PowerButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnPowerButtonPressed.Raise(this);
		}

		private void ExitButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnExitButtonPressed.Raise(this);
		}

		private void GuideButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnGuideButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}

		private void MenuDirectionsOnButtonPressed(object sender, DPadEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case DPadEventArgs.eDirection.Up:
					OnUpButtonPressed.Raise(this);
					break;
				
				case DPadEventArgs.eDirection.Down:
					OnDownButtonPressed.Raise(this);
					break;
				
				case DPadEventArgs.eDirection.Left:
					OnLeftButtonPressed.Raise(this);
					break;
				
				case DPadEventArgs.eDirection.Right:
					OnRightButtonPressed.Raise(this);
					break;
				
				case DPadEventArgs.eDirection.Center:
					OnSelectButtonPressed.Raise(this);
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void NumberKeypadOnButtonPressed(object sender, SimpleKeypadEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case SimpleKeypadEventArgs.eButton.Zero:
				case SimpleKeypadEventArgs.eButton.One:
				case SimpleKeypadEventArgs.eButton.Two:
				case SimpleKeypadEventArgs.eButton.Three:
				case SimpleKeypadEventArgs.eButton.Four:
				case SimpleKeypadEventArgs.eButton.Five:
				case SimpleKeypadEventArgs.eButton.Six:
				case SimpleKeypadEventArgs.eButton.Seven:
				case SimpleKeypadEventArgs.eButton.Eight:
				case SimpleKeypadEventArgs.eButton.Nine:
					char value = m_NumberKeypad.GetButtonChar(eventArgs.Data);
					OnNumberButtonPressed.Raise(this, new CharEventArgs(value));
					break;

				case SimpleKeypadEventArgs.eButton.MiscButtonOne:
					OnClearButtonPressed.Raise(this);
					break;
				
				case SimpleKeypadEventArgs.eButton.MiscButtonTwo:
					OnEnterButtonPressed.Raise(this);
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ChannelUpButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnChannelUpButtonPressed.Raise(this);
		}

		private void ChannelDownButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnChannelDownButtonPressed.Raise(this);
		}

		private void PageUpButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnPageUpButtonPressed.Raise(this);
		}

		private void PageDownButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnPageDownButtonPressed.Raise(this);
		}

		#endregion
	}
}
