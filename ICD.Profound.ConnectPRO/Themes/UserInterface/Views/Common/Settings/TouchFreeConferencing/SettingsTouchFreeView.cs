using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.TouchFreeConferencing
{
	[ViewBinding(typeof(ISettingsTouchFreeView))]
	public sealed partial class SettingsTouchFreeView : AbstractUiView, ISettingsTouchFreeView
	{
		public event EventHandler OnCountDownTimerIncrementButtonPressed;
		public event EventHandler OnCountDownTimerDecrementButtonPressed;
		public event EventHandler OnIncrementDecrementButtonReleased;
		public event EventHandler OnEnableZeroTouchTogglePressed;

		private readonly List<IReferencedSettingsTouchFreeView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsTouchFreeView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedSettingsTouchFreeView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCountDownTimerIncrementButtonPressed = null;
			OnCountDownTimerDecrementButtonPressed = null;
			OnIncrementDecrementButtonReleased = null;
			OnEnableZeroTouchTogglePressed = null;

			base.Dispose();
		}

		#region Methods

		public void SetTouchFreeToggleSelected(bool selected)
		{
			// Zero Touch Toggle Button graphic defaults to selected
			m_EnableZeroTouchToggleButton.SetSelected(!selected);

			// Set the enabled state of the controls
			m_CountDownSecondsLabel.Enable(selected);
		}

		public void SetCountDownSeconds(int seconds)
		{
			m_CountDownSecondsLabel.SetLabelText(seconds.ToString());
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedSettingsTouchFreeView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_DefaultDeviceList, m_ChildList, count);
		}

		#endregion

		#region Control Callbacks 

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CountDownTimerIncrementButton.OnPressed += CountDownTimerIncrementButtonOnPressed;
			m_CountDownTimerIncrementButton.OnReleased += CountDownTimerIncrementButtonOnReleased;
			m_CountDownTimerDecrementButton.OnPressed += CountDownTimerDecrementButtonOnPressed;
			m_CountDownTimerDecrementButton.OnReleased += CountDownTimerDecrementButtonOnReleased;
			m_EnableZeroTouchToggleButton.OnPressed += EnableZeroTouchToggleButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes to the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CountDownTimerIncrementButton.OnPressed -= CountDownTimerIncrementButtonOnPressed;
			m_CountDownTimerIncrementButton.OnReleased -= CountDownTimerIncrementButtonOnReleased;
			m_CountDownTimerDecrementButton.OnPressed -= CountDownTimerDecrementButtonOnPressed;
			m_CountDownTimerDecrementButton.OnReleased -= CountDownTimerDecrementButtonOnReleased;
			m_EnableZeroTouchToggleButton.OnPressed -= EnableZeroTouchToggleButtonOnPressed;
		}

		private void EnableZeroTouchToggleButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnEnableZeroTouchTogglePressed.Raise(this);
		}

		private void CountDownTimerDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCountDownTimerDecrementButtonPressed.Raise(this);
		}

		private void CountDownTimerIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCountDownTimerIncrementButtonPressed.Raise(this);
		}

		private void CountDownTimerDecrementButtonOnReleased(object sender, EventArgs eventArgs)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		private void CountDownTimerIncrementButtonOnReleased(object sender, EventArgs eventArgs)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		#endregion
	}
}