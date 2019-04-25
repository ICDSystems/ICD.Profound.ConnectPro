using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Conference
{
	[ViewBinding(typeof(IOsdConferenceView))]
	public sealed partial class OsdConferenceView : AbstractOsdView, IOsdConferenceView
	{
		public OsdConferenceView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetCurrentBookingTimeText(string text)
		{
			m_CurrentBookingTimeLabel.SetLabelText(text);
		}

		public void SetCurrentBookingTimeVisibility(bool visible)
		{
			m_CurrentBookingTimeLabel.Show(visible);
		}

		public void SetCurrentBookingNameText(string text)
		{
			m_CurrentBookingNameLabel.SetLabelText(text);
		}

		public void SetCurrentBookingPanelVisibility(bool visible)
		{
			// TODO: hack, fix when there's OSD UI controls
			// name is always visible, so instead we use this join to set visibility
			// of the meeting panel if there's no scheduler for the room
			m_CurrentBookingNameLabel.Show(visible);
		}

		public void SetCurrentBookingHostText(string text)
		{
			m_CurrentBookingHostLabel.SetLabelText(text);
		}

		public void SetSourceIcon(string icon)
		{
			m_SourceIcon.SetIcon(icon);
		}

		public void SetSourceDescription(string description)
		{
			m_SourceDescriptionLabel.SetLabelText(description);
		}

		public void SetConnectingBannerVisibility(bool visible)
		{
			m_ConnectingBanner.Show(visible);
		}
	}
}
