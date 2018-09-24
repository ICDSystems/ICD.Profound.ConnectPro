using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class ReferencedSchedulePresenter : AbstractComponentPresenter<IReferencedScheduleView>,
														  IReferencedSchedulePresenter
	{
		/// <summary>
		/// Raised when the user presses the presenter.
		/// </summary>
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;
		private IBooking m_Booking;
		private bool m_Selected;

		#region Properties

		/// <summary>
		/// Gets/sets the source for the presenter.
		/// </summary>
		public IBooking Booking
		{
			get { return m_Booking; }
			set
			{
				if (value == m_Booking)
					return;

				m_Booking = value;

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedSchedulePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		public void SetSelected(bool selected)
		{
			m_Selected = selected;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedScheduleView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{

				string meetingTypeIcon;
				switch (Booking.Type)
				{
					case eMeetingType.VideoConference:
						meetingTypeIcon = "videoConference";
						break;
					case eMeetingType.AudioConference:
						meetingTypeIcon = "audioConference";
						break;
					case eMeetingType.Presentation:
					default:
						meetingTypeIcon = "display";
						break;
				}
				string icon = Icons.GetSourceIcon(meetingTypeIcon, eSourceColor.Grey);
				view.SetBookingIcon(icon);

				view.SetStartTimeLabel(m_Booking.StartTime.ToShortTimeString());
				if (m_Booking.IsPrivate)
				{ 
					view.SetBodyLabel("Private Meeting");
				}
				else
				{
					view.SetBodyLabel(m_Booking.MeetingName);
				}
				view.SetEndTimeLabel(m_Booking.EndTime.ToShortTimeString());
				view.SetPresenterNameLabel(m_Booking.OrganizerName);
				view.SetSelected(m_Selected);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IReferencedScheduleView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedScheduleView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the source button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
