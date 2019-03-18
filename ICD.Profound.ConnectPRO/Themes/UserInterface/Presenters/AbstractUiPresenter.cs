﻿using System;
using ICD.Common.Properties;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	/// <summary>
	/// Base class for all presenters.
	/// </summary>
	public abstract class AbstractUiPresenter<T> : AbstractPresenter<T>, IUiPresenter<T>
		where T : class, IUiView
	{
		private readonly ConnectProTheme m_Theme;

		#region Properties

		/// <summary>
		/// Gets the room.
		/// </summary>
		[CanBeNull]
		public IConnectProRoom Room { get; private set; }

		/// <summary>
		/// Gets the parent theme.
		/// </summary>
		protected ConnectProTheme Theme { get { return m_Theme; } }

		/// <summary>
		/// Returns true if this presenter is part of a collection of components.
		/// </summary>
		public override bool IsComponent { get { return false; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractUiPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views)
		{
			if (theme == null)
				throw new ArgumentNullException("theme");

			m_Theme = theme;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			SetRoom(null);

			base.Dispose();
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public virtual void SetRoom(IConnectProRoom room)
		{
			if (room == Room)
				return;

			Unsubscribe(Room);
			Room = room;
			Subscribe(Room);

			Refresh();
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Subscribe(IConnectProRoom room)
		{
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Unsubscribe(IConnectProRoom room)
		{
		}

		#endregion
	}
}