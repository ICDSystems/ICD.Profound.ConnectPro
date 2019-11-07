using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	[ViewBinding(typeof(IGenericAlertView))]
	public sealed partial class GenericAlertView : AbstractUiView, IGenericAlertView
	{
		/// <summary>
		/// Raised when the user presses a button.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public GenericAlertView(ISigInputOutput panel, ConnectProTheme theme)
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

		#region Methods

		/// <summary>
		/// Sets the text prompt.
		/// </summary>
		/// <param name="text"></param>
		public void SetAlertText(string text)
		{
			text = HtmlUtils.ReplaceNewlines(text);
			text = HtmlUtils.FormatFontSize(text, 25);

			m_AlertMessageLabel.SetLabelText(text);
		}

		/// <summary>
		/// Sets the number of buttons and their labels.
		/// </summary>
		/// <param name="buttons"></param>
		public void SetButtons(IEnumerable<string> buttons)
		{
			if (buttons == null)
				throw new ArgumentNullException("buttons");

			IList<string> cast = buttons as IList<string> ?? buttons.ToArray();
			m_Buttons.SetItemLabels(cast);
		}

		/// <summary>
		/// Sets the enabled state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		public void SetButtonEnabled(ushort index, bool enabled)
		{
			m_Buttons.SetItemEnabled(index, enabled);
		}

		/// <summary>
		/// Sets the visibility of button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetButtonVisible(ushort index, bool visible)
		{
			m_Buttons.SetItemVisible(index, visible);
		}

		/// <summary>
		/// Sets the selection state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetButtonSelected(ushort index, bool selected)
		{
			m_Buttons.SetItemSelected(index, selected);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Buttons.OnButtonClicked += ButtonsOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Buttons.OnButtonClicked -= ButtonsOnButtonClicked;
		}

		/// <summary>
		/// Called when the user presses a button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ButtonsOnButtonClicked(object sender, UShortEventArgs eventArgs)
		{
			OnButtonPressed.Raise(this, new UShortEventArgs(eventArgs.Data));
		}

		#endregion
	}
}