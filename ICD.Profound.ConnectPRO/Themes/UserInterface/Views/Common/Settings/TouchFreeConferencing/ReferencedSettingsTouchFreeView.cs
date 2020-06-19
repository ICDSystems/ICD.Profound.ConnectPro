using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.TouchFreeConferencing
{
	[ViewBinding(typeof(IReferencedSettingsTouchFreeView))]
	public sealed partial class ReferencedSettingsTouchFreeView : AbstractComponentView, IReferencedSettingsTouchFreeView
	{
		public event EventHandler OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ReferencedSettingsTouchFreeView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
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

		#region Methods

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		public void SetIcon(string icon)
		{
			m_Icon.SetIcon(icon);
		}

		/// <summary>
		/// Sets the text for the label.
		/// </summary>
		/// <param name="text"></param>
		public void SetText(string text)
		{
			m_Text.SetLabelText(text);
		}

		/// <summary>
		/// Sets the selected state of the button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetSelected(bool selected)
		{
			m_Device.SetSelected(selected);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Device.OnPressed += DeviceOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Device.OnPressed -= OnButtonPressed;
		}

		private void DeviceOnPressed(object sender, EventArgs eventArgs)
		{
			OnButtonPressed.Raise(this);
		}

		#endregion
	}
}