using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference
{
	[ViewBinding(typeof(IShareConferenceView))]
	public sealed partial class ShareConferenceView : AbstractTouchDisplayView, IShareConferenceView
	{
		
		public event EventHandler OnShareButtonPressed;
		public event EventHandler<UShortEventArgs> OnSourceButtonPressed;

		public ShareConferenceView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public override void Dispose()
		{
			base.Dispose();

			OnSourceButtonPressed = null;
			OnShareButtonPressed = null;
		}

		#region Methods

		public void SetButtonLabel(ushort index, string label)
		{
			m_SourceList.SetItemLabel(index, label);
		}

		public void SetButtonIcon(ushort index, string icon)
		{
			m_SourceList.SetItemIcon(index, icon);
		}

		public void SetButtonSelected(ushort index, bool selected)
		{
			m_SourceList.SetItemSelected(index, selected);
		}

		public void SetButtonCount(ushort count)
		{
			m_SourceList.SetNumberOfItems(count);
		}

		public void SetShareButtonEnabled(bool enabled)
		{
			m_ShareButton.Enable(enabled);
		}

		public void SetShareButtonSelected(bool selected)
		{
			m_ShareButton.SetSelected(selected);
		}

		public void SetShareButtonText(string text)
		{
			m_ShareButton.SetLabelText(text);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_SourceList.OnButtonClicked += SourceListOnOnButtonClicked;
			m_ShareButton.OnPressed += ShareButtonOnOnPressed;
		}

		private void SourceListOnOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnSourceButtonPressed.Raise(this, new UShortEventArgs(args.Data));
		}

		private void ShareButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnShareButtonPressed.Raise(this);
		}

		#endregion
	}
}
