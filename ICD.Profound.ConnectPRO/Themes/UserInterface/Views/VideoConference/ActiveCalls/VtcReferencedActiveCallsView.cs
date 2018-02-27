using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.ActiveCalls
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
		/// Sets the label text for the contact.
		/// </summary>
		/// <param name="label"></param>
		public void SetLabel(string label)
		{
			m_Label.SetLabelText(label);
		}

		/// <summary>
		/// Sets the visibility of the hangup button.
		/// </summary>
		/// <param name="visible"></param>
		public void SetHangupButtonVisible(bool visible)
		{
			m_HangupButton.Show(visible);
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
