using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Header
{
	[ViewBinding(typeof(IReferencedHeaderButtonView))]
	public sealed partial class ReferencedHeaderButtonView : AbstractTouchDisplayComponentView, IReferencedHeaderButtonView
	{
		public event EventHandler OnPressed;

		public ReferencedHeaderButtonView(ISigInputOutput panel, TouchCueTheme theme, IVtProParent parent, ushort index) : base(panel, theme, parent, index)
		{
		}
		public void SetLabelText(string text)
		{
			m_Button.SetLabelText(text);
		}

		public void SetIcon(string icon)
		{
			m_Icon.SetIcon(icon);
		}

		public void SetButtonMode(eHeaderButtonMode mode)
		{
			m_Button.SetMode((ushort)mode);
		}

		public void SetButtonEnabled(bool enabled)
		{
			m_Button.Enable(enabled);
		}

		public void SetButtonBackgroundVisible(bool visible)
		{
			m_Background.Show(visible);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Button.OnPressed += ButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.SubscribeControls();

			m_Button.OnPressed -= ButtonOnPressed;
		}

		private void ButtonOnPressed(object sender, EventArgs e)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}