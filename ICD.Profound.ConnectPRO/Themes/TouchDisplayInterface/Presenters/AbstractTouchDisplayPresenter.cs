using System;
using ICD.Common.Properties;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters
{
	public class AbstractTouchDisplayPresenter<T> : AbstractPresenter<T>, ITouchDisplayPresenter
		where T : class, ITouchDisplayView
	{
		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public AbstractTouchDisplayPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) :
			base(nav, views)
		{
			if (theme == null)
				throw new ArgumentNullException("theme");

			Theme = theme;
		}

		/// <summary>
		///     Gets the room.
		/// </summary>
		[CanBeNull]
		public IConnectProRoom Room { get; private set; }

		/// <summary>
		///     Gets the parent theme.
		/// </summary>
		protected ConnectProTheme Theme { get; private set; }

		#region Methods

		/// <summary>
		///     Release resources.
		/// </summary>
		public override void Dispose()
		{
			SetRoom(null);

			base.Dispose();
		}

		/// <summary>
		///     Sets the room for this presenter to represent.
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
		///     Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Subscribe(IConnectProRoom room)
		{
		}

		/// <summary>
		///     Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Unsubscribe(IConnectProRoom room)
		{
		}

		#endregion
	}
}