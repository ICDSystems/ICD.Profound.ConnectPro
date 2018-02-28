using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcCallListToggleView : AbstractView, IVtcCallListToggleView
	{
		public event EventHandler OnButtonPressed;

		public void SetContactsMode(bool mode)
		{
			m_Button.SetSelected(mode);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcCallListToggleView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnButtonPressed = null;

			base.Dispose();
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Button.OnPressed += ButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Button.OnPressed -= ButtonOnPressed;
		}

		private void ButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnButtonPressed.Raise(this);
		}

		#endregion
	}
}