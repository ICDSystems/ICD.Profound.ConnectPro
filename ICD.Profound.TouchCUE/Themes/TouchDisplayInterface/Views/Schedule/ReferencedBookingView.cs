using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Schedule
{
	[ViewBinding(typeof(IReferencedBookingView))]
	public sealed partial class ReferencedBookingView : AbstractTouchDisplayComponentView, IReferencedBookingView
	{
		public ReferencedBookingView(ISigInputOutput panel, TouchCueTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		public event EventHandler OnPressed;

		public void SetTimeLabel(string text)
		{
			m_TimeLabel.SetLabelText(text);
		}

		public void SetSubjectLabel(string text)
		{
			m_SubjectLabel.SetLabelText(text);
		}

		public void SetButtonEnabled(bool enabled)
		{
			m_Button.Enable(enabled);
		}

		public void SetButtonSelected(bool selected)
		{
			m_Button.SetSelected(selected);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Button.OnPressed += ButtonOnPressed;
		}

		private void ButtonOnPressed(object sender, EventArgs e)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}