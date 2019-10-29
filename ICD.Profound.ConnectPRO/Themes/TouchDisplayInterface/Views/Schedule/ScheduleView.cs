using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Schedule
{
	[ViewBinding(typeof(IScheduleView))]
	public sealed partial class ScheduleView : AbstractTouchDisplayView, IScheduleView
	{
		public event EventHandler OnCloseButtonPressed;
		public event EventHandler OnStartBookingButtonPressed;

		private readonly List<IReferencedBookingView> m_ChildList;

		public ScheduleView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedBookingView>();
		}

		public void SetAvailabilityText(string text)
		{
			m_RoomAvailabilityLabel.SetLabelText(text);
		}

		public void SetAvailabilityVisible(bool visible)
		{
			m_RoomAvailabilityLabel.Show(visible);
		}

		public void SetCurrentBookingIcon(string icon)
		{
			m_CurrentBookingIcon.SetIcon(icon);
		}

		public void SetCurrentBookingTime(string time)
		{
			m_CurrentBookingTimeLabel.SetLabelText(time);
		}

		public void SetCurrentBookingSubject(string meetingName)
		{
			m_CurrentBookingNameLabel.SetLabelText(meetingName);
		}

		public void SetColorMode(eScheduleViewColorMode mode)
		{
			m_StartBookingButton.SetMode((ushort)mode);
		}
		
		public void SetCloseButtonVisible(bool visible)
		{
			m_CloseBookingButton.Show(visible);
		}

		public void SetStartBookingButtonVisible(bool visible)
		{
			m_StartBookingButton.Show(visible);
		}
		public void SetStartBookingButtonSelected(bool selected)
		{
			m_StartBookingButton.SetSelected(selected);
		}
		public void SetStartBookingButtonText(string text)
		{
			m_StartBookingButton.SetLabelText(text);
		}

		/// <summary>
		///     Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedBookingView> GetChildComponentViews(ITouchDisplayViewFactory factory,
			ushort count)
		{
			return GetChildViews(factory, m_ScheduleList, m_ChildList, count);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseBookingButton.OnPressed += CloseBookingButtonOnPressed;
			m_StartBookingButton.OnPressed += StartBookingButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseBookingButton.OnPressed -= CloseBookingButtonOnPressed;
			m_StartBookingButton.OnPressed -= StartBookingButtonOnPressed;
		}

		private void StartBookingButtonOnPressed(object sender, EventArgs e)
		{
			OnStartBookingButtonPressed.Raise(this);
		}

		private void CloseBookingButtonOnPressed(object sender, EventArgs e)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}