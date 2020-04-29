using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference.ActiveConference
{
	[ViewBinding(typeof(IParticipantControlsView))]
	public sealed partial class ParticipantControlsView : AbstractTouchDisplayView, IParticipantControlsView
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
		public ParticipantControlsView(ISigInputOutput panel, TouchCueTheme theme)
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

		/// <summary>
		/// Sets the icon for a button in the list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		public void SetButtonIcon(ushort index, string icon)
		{
			m_ButtonList.SetItemIcon(index, icon);
		}

		/// <summary>
		/// Sets the label for a button in the list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		public void SetButtonLabel(ushort index, string label)
		{
			m_ButtonList.SetItemLabel(index, label);
		}

		/// <summary>
		/// Sets the visibility of a button in the list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetButtonVisible(ushort index, bool visible)
		{
			m_ButtonList.SetItemVisible(index, visible);
		}

		/// <summary>
		/// Sets the enabled state of a button in the list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		public void SetButtonEnabled(ushort index, bool enabled)
		{
			m_ButtonList.SetItemEnabled(index, enabled);
		}

		/// <summary>
		/// Sets the selected state of a button in the list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetButtonSelected(ushort index, bool selected)
		{
			m_ButtonList.SetItemSelected(index, selected);
		}

		public void SetNumberOfItems(ushort numItems)
		{
			m_ButtonList.SetNumberOfItems(numItems);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ButtonList.OnButtonClicked += ButtonListOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ButtonList.OnButtonClicked -= ButtonListOnButtonClicked;
		}

		/// <summary>
		/// Called when the user presses a button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonListOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		#endregion
	}
}