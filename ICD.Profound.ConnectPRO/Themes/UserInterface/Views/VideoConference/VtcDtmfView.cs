﻿using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcDtmfView : AbstractView, IVtcDtmfView
	{
		public event EventHandler<CharEventArgs> OnToneButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcDtmfView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Keypad.OnButtonPressed += KeypadOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Keypad.OnButtonPressed -= KeypadOnButtonPressed;
		}

		private void KeypadOnButtonPressed(object sender, SimpleKeypadEventArgs eventArgs)
		{
			char key = m_Keypad.GetButtonChar(eventArgs.Data);
			OnToneButtonPressed.Raise(this, new CharEventArgs(key));
		}
	}
}
