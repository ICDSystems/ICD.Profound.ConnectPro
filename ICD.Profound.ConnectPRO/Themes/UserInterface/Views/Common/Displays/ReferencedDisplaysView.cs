using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public sealed partial class ReferencedDisplaysView : AbstractComponentView, IReferencedDisplaysView
	{
		private const ushort MODE_GREY = 0;
		private const ushort MODE_YELLOW = 1;
		private const ushort MODE_GREEN = 2;

		public event EventHandler OnButtonPressed;
		public event EventHandler OnSpeakerButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ReferencedDisplaysView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
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
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		public void SetColor(eDisplayColor color)
		{
			switch (color)
			{
				case eDisplayColor.Grey:
					m_BackgroundButton.SetMode(MODE_GREY);
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

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		public void SetIcon(string icon)
		{
			m_Icon.SetIcon(icon);
		}

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		public void SetLine1Text(string text)
		{
			m_Line1Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		public void SetLine2Text(string text)
		{
			m_Line2Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets the visibility of the speaker button.
		/// </summary>
		/// <param name="visible"></param>
		public void ShowSpeakerButton(bool visible)
		{
			m_SpeakerButton.Show(visible);
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
			m_SpeakerButton.OnPressed += SpeakerButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_BackgroundButton.OnPressed -= BackgroundButtonOnPressed;
			m_SpeakerButton.OnPressed -= SpeakerButtonOnPressed;
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

		/// <summary>
		/// Called when the user presses the speaker button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SpeakerButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSpeakerButtonPressed.Raise(this);
		}

		#endregion
	}
}
