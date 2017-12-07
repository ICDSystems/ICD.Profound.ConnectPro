using System;
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
			bool grey = color == eDisplayColor.Grey;
			bool green = color == eDisplayColor.Green;
			bool yellow = color == eDisplayColor.Yellow;

			m_GreyIconButton.Show(grey);
			m_GreyLine1Label.Show(grey);
			m_GreyLine2Label.Show(grey);

			m_GreenIconButton.Show(green);
			m_GreenLine1Label.Show(green);
			m_GreenLine2Label.Show(green);

			m_YellowIconButton.Show(yellow);
			m_YellowLine1Label.Show(yellow);
			m_YellowLine2Label.Show(yellow);

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
		public void SetIcon(IDisplayIcon icon)
		{
			ushort mode = icon.Mode;

			m_GreenIconButton.SetMode(mode);
			m_YellowIconButton.SetMode(mode);
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
			m_YellowLine1Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		public void SetLine2Text(string text)
		{
			m_GreyLine2Label.SetLabelText(text);
			m_GreenLine2Label.SetLabelText(text);
			m_YellowLine2Label.SetLabelText(text);
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
