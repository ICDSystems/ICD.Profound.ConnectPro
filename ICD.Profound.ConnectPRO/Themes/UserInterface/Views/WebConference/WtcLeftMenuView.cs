using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public sealed partial class WtcLeftMenuView : AbstractUiView, IWtcLeftMenuView
	{
		private const ushort SUBPAGE_3 = 763;
		private const ushort SUBPAGE_4 = 764;
		private const ushort SUBPAGE_5 = 765;

		private static ushort[] SUBPAGE_JOINS = {
			SUBPAGE_3,
			SUBPAGE_4,
			SUBPAGE_5
		};

		public event EventHandler<UShortEventArgs> OnButtonPressed;

		private ushort m_ButtonCount;

		public WtcLeftMenuView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
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
		/// Sets the icon for a button in the list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		public void SetButtonIcon(ushort index, string icon)
		{
			m_ButtonList.SetItemIcon(index, icon);
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
		/// Sets the selected state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetButtonSelected(ushort index, bool selected)
		{
			m_ButtonList.SetItemSelected(index, selected);
		}

		public void SetButtonCount(ushort count)
		{
			m_ButtonCount = count;
			UpdateSubpageVisibility();
			m_ButtonList.SetNumberOfItems(count);
		}

		public void SetActiveMeetingIndicatorMode(bool inMeeting)
		{
			m_ActiveMeetingIndicator.SetMode((ushort)(inMeeting ? 1 : 0));
		}

		public override void Show(bool visible)
		{
			base.Show(visible);

			UpdateSubpageVisibility();
		}

		private void UpdateSubpageVisibility()
		{
			ushort visible;
			if (m_ButtonCount <= 3)
				visible = SUBPAGE_3;
			else if (m_ButtonCount == 4)
				visible = SUBPAGE_4;
			else
				visible = SUBPAGE_5;

			foreach (var subpage in SUBPAGE_JOINS)
				Panel.SendInputDigital(subpage, subpage == visible && IsVisible);
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