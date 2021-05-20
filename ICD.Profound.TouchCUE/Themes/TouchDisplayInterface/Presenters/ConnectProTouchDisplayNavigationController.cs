using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters
{
	public sealed class ConnectProTouchDisplayNavigationController : AbstractNavigationController,
		ITouchDisplayNavigationController
	{
		private readonly SafeCriticalSection m_SetRoomSection;
		private readonly TouchCueTheme m_Theme;
		private readonly ITouchDisplayViewFactory m_ViewFactory;

		private IConnectProRoom m_Room;

		#region Constructors

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="viewFactory"></param>
		/// <param name="theme"></param>
		public ConnectProTouchDisplayNavigationController(ITouchDisplayViewFactory viewFactory, TouchCueTheme theme)
		{
			m_ViewFactory = viewFactory;
			m_Theme = theme;
			m_SetRoomSection = new SafeCriticalSection();
		}

		#endregion

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
		///     Updates the presenters to track the given room.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			m_SetRoomSection.Enter();

			try
			{
				if (room == m_Room)
					return;

				m_Room = room;

				foreach (var presenter in GetPresenters().OfType<ITouchDisplayPresenter>())
					presenter.SetRoom(m_Room);
			}
			finally
			{
				m_SetRoomSection.Leave();
			}
		}

		/// <summary>
		///     Instantiates a new presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public override IPresenter GetNewPresenter(Type type)
		{
			var output =
				(ITouchDisplayPresenter) GetNewPresenter(type, this, m_ViewFactory, m_Theme);
			if (!output.GetType().IsAssignableTo(type))
				throw new InvalidCastException(string.Format("Presenter {0} is not of type {1}", output, type.Name));

			output.SetRoom(m_Room);

			return output;
		}

		#endregion
	}
}