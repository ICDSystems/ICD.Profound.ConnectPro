using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	[ViewBinding(typeof(IReferencedScheduleView))]
	public sealed partial class ReferencedScheduleView : AbstractComponentView, IReferencedScheduleView
	{
		public event EventHandler OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ReferencedScheduleView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		#region Methods

		/// <summary>
		/// Sets the booking icon.
		/// </summary>
		/// <param name="icon"></param>
		public void SetBookingIcon(string icon)
		{
			m_BookingIcon.SetIcon(icon);
		}

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		public void SetStartTimeLabel(string text)
		{
			m_StartTimeLabel.SetLabelText(text);
		}

		/// <summary>
		/// Sets the text for the third label.
		/// </summary>
		/// <param name="text"></param>
		public void SetBodyLabel(string text)
		{
			m_BodyLabel.SetLabelText(text);
		}

		/// <summary>
		/// Sets the text for the forth label.
		/// </summary>
		/// <param name="text"></param>
		public void SetEndTimeLabel(string text)
		{
			m_EndTimeLabel.SetLabelText(text);
		}

		/// <summary>
		/// Sets the text for the fifth label.
		/// </summary>
		/// <param name="text"></param>
		public void SetPresenterNameLabel(string text)
		{
			m_PresenterNameLabel.SetLabelText(text);
		}

		/// <summary>
		/// Sets the background button selected state.
		/// </summary>
		/// <param name="selected"></param>
		public void SetSelected(bool selected)
		{
			m_BackgroundButton.SetSelected(selected);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_BackgroundButton.OnPressed += BackgroundButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_BackgroundButton.OnPressed -= BackgroundButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void BackgroundButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnButtonPressed.Raise(this);
		}

		#endregion
	}
}
