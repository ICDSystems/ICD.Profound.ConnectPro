using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	[ViewBinding(typeof(IReferencedDisplayView))]
	public sealed partial class ReferencedDisplayView : AbstractUiView, IReferencedDisplayView
	{
		private const ushort MODE_GREY = 0;
		private const ushort MODE_WHITE = 1;
		private const ushort MODE_YELLOW = 2;
		private const ushort MODE_GREEN = 3;

		public event EventHandler OnDisplayButtonPressed;
		public event EventHandler OnDisplaySpeakerButtonPressed;

		public ReferencedDisplayView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index) : base(panel, theme, parent, index)
		{
		}

		public void SetDisplayColor(eDisplayColor color)
		{
			switch (color)
			{
				case eDisplayColor.Grey:
					m_BackgroundButton.SetMode(MODE_GREY);
					break;
				case eDisplayColor.White:
					m_BackgroundButton.SetMode(MODE_WHITE);
					break;
				case eDisplayColor.Yellow:
					m_BackgroundButton.SetMode(MODE_YELLOW);
					break;
				case eDisplayColor.Green:
					m_BackgroundButton.SetMode(MODE_GREEN);
					break;
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}

		public void SetDisplayIcon(string icon)
		{
			m_DisplayIcon.SetIcon(icon);
		}

		public void SetDisplayLine1Text(string text)
		{
			m_Line1Label.SetLabelText(text);
		}

		public void SetDisplayLine2Text(string text)
		{
			m_Line2Label.SetLabelText(text);
		}

		public void ShowDisplaySpeakerButton(bool visible)
		{
			m_SpeakerButton.Show(visible);
		}

		public void SetDisplaySpeakerButtonActive(bool active)
		{
			m_SpeakerButton.SetSelected(!active);
		}

		public void SetDisplaySourceText(string text)
		{
			m_SourceLabel.SetLabelText(text);
		}

		public void SetWarmupStatusText(string text)
		{
			m_WarmupLabel.SetLabelText(text);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_BackgroundButton.OnPressed += BackgroundButtonOnPressed;
			m_SpeakerButton.OnPressed += SpeakerButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_BackgroundButton.OnPressed -= BackgroundButtonOnPressed;
			m_SpeakerButton.OnPressed -= SpeakerButtonOnPressed;
		}

		private void BackgroundButtonOnPressed(object sender, EventArgs args)
		{
			OnDisplayButtonPressed.Raise(this);
		}

		private void SpeakerButtonOnPressed(object sender, EventArgs args)
		{
			OnDisplaySpeakerButtonPressed.Raise(this);
		}

		#endregion
	}
}
