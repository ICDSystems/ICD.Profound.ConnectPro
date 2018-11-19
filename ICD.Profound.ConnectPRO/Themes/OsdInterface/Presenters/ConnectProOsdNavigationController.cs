#if !SIMPLSHARP
using System.Reflection;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Sources;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Welcome;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Popups;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Sources;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Welcome;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	/// <summary>
	/// Provides a way for presenters to access each other.
	/// </summary>
	public sealed class ConnectProOsdNavigationController : AbstractNavigationController, IOsdNavigationController
	{
		private delegate IOsdPresenter PresenterFactory(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme);

		private readonly Dictionary<Type, PresenterFactory> m_PresenterFactories = new Dictionary<Type, PresenterFactory>
		{
			{typeof(IOsdHeaderPresenter), (nav, views, theme) => new OsdHeaderPresenter(nav, views, theme)},
			{typeof(IOsdWelcomePresenter), (nav, views, theme) => new OsdWelcomePresenter(nav, views, theme)},
			{typeof(IOsdSourcesPresenter), (nav, views, theme) => new OsdSourcesPresenter(nav, views, theme)},
			
			{typeof(IOsdHelloPresenter), (nav, views, theme) => new OsdHelloPresenter(nav, views, theme)},
			{typeof(IOsdIncomingCallPresenter), (nav, views, theme) => new OsdIncomingCallPresenter(nav, views, theme)},
			{typeof(IOsdMutePresenter), (nav, views, theme) => new OsdMutePresenter(nav, views, theme)},

            {typeof(IReferencedSchedulePresenter), (nav, views, theme) => new ReferencedSchedulePresenter(nav, views, theme)},
		};

		private readonly IOsdViewFactory m_ViewFactory;
		private readonly ConnectProTheme m_Theme;
		private readonly SafeCriticalSection m_SetRoomSection;

		private IConnectProRoom m_Room;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="viewFactory"></param>
		/// <param name="theme"></param>
		public ConnectProOsdNavigationController(IOsdViewFactory viewFactory, ConnectProTheme theme)
		{
			m_ViewFactory = viewFactory;
			m_Theme = theme;
			m_SetRoomSection = new SafeCriticalSection();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Updates the presenters to track the given room.
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

				foreach (IOsdPresenter presenter in GetPresenters().OfType<IOsdPresenter>())
					presenter.SetRoom(m_Room);
			}
			finally
			{
				m_SetRoomSection.Leave();
			}
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			SetRoom(null);

			base.Dispose();
		}

		/// <summary>
		/// Instantiates a new presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public override IPresenter GetNewPresenter(Type type)
		{
			if (!m_PresenterFactories.ContainsKey(type))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, type.Name);
				throw new KeyNotFoundException(message);
			}

			PresenterFactory factory = m_PresenterFactories[type];
			IOsdPresenter output = factory(this, m_ViewFactory, m_Theme);
			output.SetRoom(m_Room);

			if (!type
#if !SIMPLSHARP
				     .GetTypeInfo()
#endif
				     .IsInstanceOfType(output))
				throw new InvalidCastException(string.Format("Presenter {0} is not of type {1}", output, type.Name));

			return output;
		}

		#endregion
	}
}
