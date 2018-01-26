﻿using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcBasePresenter : AbstractPopupPresenter<IVtcBaseView>, IVtcBasePresenter
	{
		private const ushort INDEX_CONTACTS = 0;
		private const ushort INDEX_CAMERA = 1;
		private const ushort INDEX_SHARE = 2;
		private const ushort INDEX_DTMF = 3;

		private static readonly Dictionary<ushort, Type> s_NavPages =
			new Dictionary<ushort, Type>
			{
				{INDEX_CONTACTS, typeof(IVtcContactsPresenter)},
				{INDEX_CAMERA, typeof(IVtcCameraPresenter)},
				{INDEX_SHARE, typeof(IVtcSharePresenter)},
				{INDEX_DTMF, typeof(IVtcDtmfPresenter)},
			};

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcBasePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcBaseView view)
		{
			base.Subscribe(view);

			view.OnNavButtonPressed += ViewOnNavButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcBaseView view)
		{
			base.Unsubscribe(view);

			view.OnNavButtonPressed -= ViewOnNavButtonPressed;
		}

		private void ViewOnNavButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			Type type = s_NavPages[eventArgs.Data];
			Navigation.LazyLoadPresenter(type).ShowView(true);
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			// If we are in a call we want to confirm before closing
			IConferenceManager manager = Room == null ? null : Room.ConferenceManager;
			bool isInCall = manager != null && manager.IsInCall >= eInCall.Audio;
			
			if (isInCall)
				Navigation.NavigateTo<IConfirmLeaveCallPresenter>();
			else
				base.ViewOnCloseButtonPressed(sender, eventArgs);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (args.Data)
				Navigation.NavigateTo<IVtcContactsPresenter>();
			else
			{
				foreach (Type type in s_NavPages.Values)
					Navigation.LazyLoadPresenter(type).ShowView(false);

				IConferenceManager manager = Room == null ? null : Room.ConferenceManager;
				IConference active = manager == null ? null : manager.ActiveConference;

				if (active != null)
					active.Hangup();

				if (Room != null)
					Room.Routing.UnrouteVtc();
			}
		}

		#endregion
	}
}
