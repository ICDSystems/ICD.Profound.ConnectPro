namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Conference
{
	public interface IOsdConferenceView : IOsdView
	{
		void SetCurrentBookingTimeText(string text);

		void SetCurrentBookingTimeVisibility(bool visible);

		void SetCurrentBookingNameText(string text);

		void SetCurrentBookingPanelVisibility(bool visible);

		void SetCurrentBookingHostText(string text);

		void SetSourceIcon(string icon);

		void SetSourceDescription(string description);

		void SetConnectingBannerVisibility(bool visible);
	}
}
