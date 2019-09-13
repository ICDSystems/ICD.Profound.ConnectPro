﻿using System;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Schedule
{
	public sealed class ReferencedBookingPresenterFactory :
		AbstractTouchDisplayListItemFactory<IBooking, IReferencedBookingPresenter, IReferencedBookingView>
	{
		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public ReferencedBookingPresenterFactory(ITouchDisplayNavigationController navigationController,
			ListItemFactory<IReferencedBookingView> viewFactory,
			Action<IReferencedBookingPresenter> subscribe,
			Action<IReferencedBookingPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		///     Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IBooking model, IReferencedBookingPresenter presenter,
			IReferencedBookingView view)
		{
			presenter.Booking = model;
			presenter.SetView(view);
		}
	}
}