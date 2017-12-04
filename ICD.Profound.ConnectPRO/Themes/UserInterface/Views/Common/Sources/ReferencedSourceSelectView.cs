using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources
{
	public sealed partial class ReferencedSourceSelectView : AbstractComponentView, IReferencedSourceSelectView
	{
		public event EventHandler OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ReferencedSourceSelectView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		#region Methods

		/// <summary>
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		public void SetColor(eSourceColor color)
		{
			bool grey = color == eSourceColor.Grey || color == eSourceColor.Yellow || color == eSourceColor.White;
			bool green = color == eSourceColor.Green;

			m_GreyIconButton.Show(grey);
			m_GreyLine1Label.Show(grey);
			m_GreyLine2Label.Show(grey);
			m_GreyFeedbackLabel.Show(grey);

			m_GreenIconButton.Show(green);
			m_GreenLine1Label.Show(green);
			m_GreenLine2Label.Show(green);
			m_GreenFeedbackLabel.Show(green);
		}

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		public void SetIcon(ISourceIcon icon)
		{
			ushort mode = icon.Mode;

			m_GreenIconButton.SetMode(mode);
			m_GreyIconButton.SetMode(mode);
		}

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		public void SetLine1Text(string text)
		{
			m_GreyLine1Label.SetLabelText(text);
			m_GreenLine1Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		public void SetLine2Text(string text)
		{
			m_GreyLine2Label.SetLabelText(text);
			m_GreenLine2Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets the text for the feedback label.
		/// </summary>
		/// <param name="text"></param>
		public void SetFeedbackText(string text)
		{
			m_GreyFeedbackLabel.SetLabelText(text);
			m_GreenFeedbackLabel.SetLabelText(text);
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
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_BackgroundButton.OnPressed -= BackgroundButtonOnPressed;
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

		#endregion
	}
}
