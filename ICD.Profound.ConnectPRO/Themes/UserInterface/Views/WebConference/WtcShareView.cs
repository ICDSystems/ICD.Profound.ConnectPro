using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	[ViewBinding(typeof(IWtcShareView))]
	public sealed partial class WtcShareView : AbstractUiView, IWtcShareView
	{
		public event EventHandler OnShareButtonPressed;
		public event EventHandler<UShortEventArgs> OnSourceButtonPressed;

		public WtcShareView(ISigInputOutput panel, IConnectProTheme theme)
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

		public void SetSwipeLabelsVisible(bool visible)
		{
			m_SwipeLabels.Show(visible);
		}

		public void SetButtonLabel(ushort index, string label)
		{
			m_SourceList.SetItemLabel(index, label);
		}

		public void SetButtonIcon(ushort index, string icon)
		{
			m_SourceList.SetItemIcon(index, icon);
		}

		public void SetButtonEnabled(ushort index, bool enabled)
		{
			m_SourceList.SetItemEnabled(index, enabled);
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