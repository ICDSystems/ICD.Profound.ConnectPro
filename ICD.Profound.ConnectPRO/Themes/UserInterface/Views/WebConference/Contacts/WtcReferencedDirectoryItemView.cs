using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	public sealed partial class WtcReferencedDirectoryItemView : AbstractComponentView, IWtcReferencedDirectoryItemView
	{
		public event EventHandler OnContactPressed;

		public WtcReferencedDirectoryItemView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		public void SetContactName(string name)
		{
			m_ContactNameText.SetLabelText(name);
		}

		public void SetButtonSelected(bool selected)
		{
			m_ContactButton.SetSelected(selected);
		}

		public override void Dispose()
		{
			OnContactPressed = null;

			base.Dispose();
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ContactButton.OnPressed += ContactButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ContactButton.OnPressed -= ContactButtonOnOnPressed;
		}

		private void ContactButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnContactPressed.Raise(this);
		}

		#endregion
	}
}