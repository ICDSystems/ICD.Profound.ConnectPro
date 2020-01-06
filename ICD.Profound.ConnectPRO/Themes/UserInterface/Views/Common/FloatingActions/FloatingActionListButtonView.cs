using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.FloatingActions
{
	[ViewBinding(typeof(IFloatingActionListButtonView))]
	public sealed partial class FloatingActionListButtonView : AbstractFloatingActionView, IFloatingActionListButtonView
	{

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public FloatingActionListButtonView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public override event EventHandler OnButtonPressed;

		/// <summary>
		/// Sets the selected state of the option.
		/// </summary>
		/// <param name="active"></param>
		public override void SetActive(bool active)
		{
			m_Button.SetSelected(active);
		}

		/// <summary>
		/// Sets the enabled state of the option.
		/// </summary>
		/// <param name="enabled"></param>
		public override void SetEnabled(bool enabled)
		{
			m_Button.Enable(enabled);
		}

		public void SetIcon(string icon)
		{
			m_Icon.SetIcon(icon);
		}

		#region Controls Callbacks

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
