using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	[ViewBinding(typeof(IVtcShareView))]
	public sealed partial class VtcShareView : AbstractUiView, IVtcShareView
	{
		public event EventHandler<UShortEventArgs> OnSourceButtonPressed;
		public event EventHandler OnShareButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcShareView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnSourceButtonPressed = null;
			OnShareButtonPressed = null;

			base.Dispose();
		}

		public void SetSwipeLabelsVisible(bool visible)
		{
			m_SwipeLabels.Show(visible);
		}

		/// <summary>
		/// Sets the label for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		public void SetButtonLabel(ushort index, string label)
		{
			m_ButtonList.SetItemLabel(index, label);
		}

		/// <summary>
		/// Sets the icon for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		public void SetButtonIcon(ushort index, string icon)
		{
			m_ButtonList.SetItemIcon(index, icon);
		}

		/// <summary>
		/// Sets the enabled state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		public void SetButtonEnabled(ushort index, bool enabled)
		{
			m_ButtonList.SetItemEnabled(index, enabled);
		}

		/// <summary>
		/// Sets the selection state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetButtonSelected(ushort index, bool selected)
		{
			m_ButtonList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the number of source buttons.
		/// </summary>
		/// <param name="count"></param>
		public void SetButtonCount(ushort count)
		{
			m_ButtonList.SetNumberOfItems(count);
		}

		/// <summary>
		/// Sets the enabled state of the share button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetShareButtonEnabled(bool enabled)
		{
			m_ShareButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the selected state of the share button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetShareButtonSelected(bool selected)
		{
			m_ShareButton.SetSelected(selected);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ButtonList.OnButtonClicked += ButtonListOnButtonClicked;
			m_ShareButton.OnPressed += ShareButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ButtonList.OnButtonClicked -= ButtonListOnButtonClicked;
			m_ShareButton.OnPressed -= ShareButtonOnPressed;
		}

		private void ShareButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnShareButtonPressed.Raise(this);
		}

		private void ButtonListOnButtonClicked(object sender, UShortEventArgs uShortEventArgs)
		{
			OnSourceButtonPressed.Raise(this, new UShortEventArgs(uShortEventArgs.Data));
		}

		#endregion
	}
}
