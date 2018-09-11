using System;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface IStartMeetingView : IView
	{
		/// <summary>
		/// Raised when the user presses the start my meeting button.
		/// </summary>
		event EventHandler OnStartMyMeetingButtonPressed;

		/// <summary>
		/// Raised when the user presses the start new meeting button.
		/// </summary>
		event EventHandler OnStartNewMeetingButtonPressed;

		/// <summary>
		/// Raised when the user presses the settings button.
		/// </summary>
		event EventHandler OnSettingsButtonPressed;

		/// <summary>
		/// Sets the enabled state of the start my meeting button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetStartMyMeetingButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the start new meeting button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetStartNewMeetingButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the image path for the logo.
		/// </summary>
		/// <param name="url"></param>
		void SetLogoPath(string url);

		/// <summary>
		/// Sets the visibility of the bookings list.
		/// </summary>
		/// <param name="visible"></param>
		void SetBookingsVisible(bool visible);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedScheduleView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
