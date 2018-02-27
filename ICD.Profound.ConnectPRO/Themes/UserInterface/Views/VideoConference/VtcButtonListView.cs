using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcButtonListView : AbstractView, IVtcButtonListView
	{
		public event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcButtonListView(ISigInputOutput panel, ConnectProTheme theme)
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
		/// Sets the labels for the buttons in the list.
		/// </summary>
		/// <param name="labels"></param>
		public void SetButtonLabels(IEnumerable<string> labels)
		{
			m_ButtonList.SetItemLabels(labels.ToArray());
		}

		/// <summary>
		/// Sets the visibility of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetButtonVisible(ushort index, bool visible)
		{
			m_ButtonList.SetItemVisible(index, visible);
		}

		public void SetButtonEnabled(ushort index, bool enabled)
		{
			m_ButtonList.SetItemEnabled(index, enabled);
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

		private void ButtonListOnButtonClicked(object sender, UShortEventArgs eventArgs)
		{
			OnButtonPressed.Raise(this, new UShortEventArgs(eventArgs.Data));
		}

		#endregion
	}
}
