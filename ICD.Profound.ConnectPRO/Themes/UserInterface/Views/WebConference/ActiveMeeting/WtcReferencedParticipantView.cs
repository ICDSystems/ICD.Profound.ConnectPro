﻿using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting
{
	public sealed partial class WtcReferencedParticipantView : AbstractComponentView, IWtcReferencedParticipantView
	{
		public event EventHandler OnParticipantPressed;

		public WtcReferencedParticipantView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		public void SetParticipantName(string name)
		{
			m_ParticipantNameText.SetLabelText(name);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ParticipantButton.OnPressed += ParticipantButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ParticipantButton.OnPressed -= ParticipantButtonOnOnPressed;
		}

		private void ParticipantButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnParticipantPressed.Raise(this);
		}

		#endregion
	}
}