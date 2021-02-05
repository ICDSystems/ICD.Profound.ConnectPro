﻿using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting
{
	[ViewBinding(typeof(IWtcReferencedParticipantView))]
	public sealed partial class WtcReferencedParticipantView : AbstractComponentView, IWtcReferencedParticipantView
	{
		public event EventHandler OnParticipantPressed;

		public WtcReferencedParticipantView(ISigInputOutput panel, IConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		public void SetParticipantName(string name)
		{
			m_ParticipantNameText.SetLabelText(name);
		}

		public void SetButtonSelected(bool selected)
		{
			m_ParticipantButton.SetSelected(selected);
		}
		
		public void SetAvatarImageVisibility(bool visible)
		{
			m_AvatarImage.Show(visible);
		}
		public void SetAvatarImagePath(string url)
		{
			m_AvatarImage.SetImageUrl(url);
		}

		public void SetMuteIconVisibility(bool visible)
		{
			m_MuteIcon.Show(visible);
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