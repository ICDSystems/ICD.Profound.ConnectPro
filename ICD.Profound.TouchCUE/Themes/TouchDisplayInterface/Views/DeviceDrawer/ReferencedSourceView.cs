using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.DeviceDrawer
{
	[ViewBinding(typeof(IReferencedSourceView))]
	public sealed partial class ReferencedSourceView : AbstractTouchDisplayComponentView, IReferencedSourceView
	{
		public event EventHandler OnButtonPressed;

		public ReferencedSourceView(ISigInputOutput panel, TouchCueTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		public void SetIcon(string icon)
		{
			m_Icon.SetIcon(icon);
		}

		public void SetNameText(string name)
		{
			m_Name.SetLabelText(name);
		}

		public void SetDescriptionText(string description)
		{
			m_Description.SetLabelText(description);
		}

		public void SetButtonMode(eDeviceButtonMode mode)
		{
			m_Button.SetMode((ushort)mode);
		}

		public void SetButtonEnabled(bool enabled)
		{
			m_Button.Enable(enabled);
		}
		
		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Button.OnPressed += ButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Button.OnPressed -= ButtonOnPressed;
		}

		private void ButtonOnPressed(object sender, EventArgs e)
		{
			OnButtonPressed.Raise(this);
		}

		#endregion
	}
}
