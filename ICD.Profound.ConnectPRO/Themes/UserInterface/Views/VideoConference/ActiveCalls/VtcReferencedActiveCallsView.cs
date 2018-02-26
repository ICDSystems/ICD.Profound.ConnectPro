using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcReferencedActiveCallsView : AbstractComponentView, IVtcReferencedActiveCallsView
	{
		public event EventHandler OnHangupButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public VtcReferencedActiveCallsView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		/// <summary>
		/// Sets the name for the contact.
		/// </summary>
		/// <param name="name"></param>
		public void SetContactName(string name)
		{
			m_ContactNameLabel.SetLabelText(name);
		}

		/// <summary>
		/// Sets the number for the contact.
		/// </summary>
		/// <param name="number"></param>
		public void SetContactNumber(string number)
		{
			m_ContactNumberLabel.SetLabelText(number);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_HangupButton.OnPressed += HangupButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_HangupButton.OnPressed -= HangupButtonOnPressed;
		}

		private void HangupButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnHangupButtonPressed.Raise(this);
		}

		#endregion
	}
}
