﻿using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom.SubSettings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Zoom.SubSettings
{
	[ViewBinding(typeof(ISettingsZoomGeneralView))]
	public sealed partial class SettingsZoomGeneralView : AbstractSettingsZoomSubView, ISettingsZoomGeneralView
	{
		/// <summary>
		/// Raised when the user presses the mute all participants button.
		/// </summary>
		public event EventHandler OnMuteAllParticipantsButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsZoomGeneralView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnMuteAllParticipantsButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the selected state of the mute all button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetMuteAllButtonSelected(bool selected)
		{
			// Toggle button feedback is reversed :(
			m_MuteParticipantsButton.SetSelected(!selected);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_MuteParticipantsButton.OnPressed += MuteParticipantsButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_MuteParticipantsButton.OnPressed -= MuteParticipantsButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the mute participants button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void MuteParticipantsButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnMuteAllParticipantsButtonPressed.Raise(this);
		}

		#endregion
	}
}