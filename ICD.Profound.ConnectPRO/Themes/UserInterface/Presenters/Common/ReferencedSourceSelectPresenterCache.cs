using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class ReferencedSchedulePresenterCache
	{

		private string m_HexColor;
		private string m_HexColor2;
		private string m_Line1Plain;
		private string m_Line2Plain;
		private string m_Line3Plain;
		private string m_Line4Plain;
		private string m_Line5Plain;

		#region Properties

		public IBooking Booking { get; private set; }
		public eScheduleColor Color1 { get; private set; }
		public eScheduleColor Color2 { get; private set; }
		public string Line1 { get; private set; }
		public string Line2 { get; private set; }
		public string Line3 { get; private set; }
		public string Line4 { get; private set; }
		public string Line5 { get; private set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ReferencedSchedulePresenterCache()
		{
			UpdateColor();
		}

		#region Methods

		public bool SetBooking(IBooking booking)
		{
			if (booking == Booking)
				return false;

			Booking = booking;

			m_Line1Plain = Booking.StartTime.Day.ToString();
			m_Line2Plain = Booking.StartTime.ToShortTimeString();
			m_Line3Plain = Booking.MeetingName;
			m_Line4Plain = Booking.EndTime.ToShortTimeString();
			m_Line5Plain = Booking.OrganizerName;

			UpdateText();

			return true;
		}

		#endregion

		#region Private Methods

		private void UpdateColor()
		{
			Color1 = eScheduleColor.Blue;
			Color2 = eScheduleColor.Red;
			m_HexColor = Colors.ScheduleColorToTextColor(Color1);
			m_HexColor2 = Colors.ScheduleColorToTextColor(Color2);

			UpdateText();
		}

		private void UpdateText()
		{
			Line1 = HtmlUtils.FormatColoredText(m_Line1Plain, m_HexColor);
			Line2 = HtmlUtils.FormatColoredText(m_Line2Plain, m_HexColor);
			Line3 = HtmlUtils.FormatColoredText(m_Line3Plain, m_HexColor);
			Line4 = HtmlUtils.FormatColoredText(m_Line4Plain, m_HexColor2);
			Line5 = HtmlUtils.FormatColoredText(m_Line5Plain, m_HexColor);
		}

		#endregion
	}
}
