#if !SIMPLSHARP
using System.Reflection;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
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
	public sealed class ConnectProOsdNavigationController : IOsdNavigationController
	{
		private delegate IOsdPresenter PresenterFactory(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme);

		private readonly Dictionary<Type, PresenterFactory> m_PresenterFactories = new Dictionary<Type, PresenterFactory>
		{
			{typeof(IOsdSourcesPresenter), (nav, views, theme) => new OsdSourcesPresenter(nav, views, theme)},
			{typeof(IOsdIncomingCallPresenter), (nav, views, theme) => new OsdIncomingCallPresenter(nav, views, theme)},
			{typeof(IOsdWelcomePresenter), (nav, views, theme) => new OsdWelcomePresenter(nav, views, theme)},
			{typeof(IHelloPresenter), (nav, views, theme) => new HelloPresenter(nav, views, theme)},

            {typeof(IReferencedSchedulePresenter), (nav, views, theme) => new ReferencedSchedulePresenter(nav, views, theme)}
		};

		private readonly Dictionary<Type, IOsdPresenter> m_Cache;
		private readonly SafeCriticalSection m_CacheSection;
		private readonly IOsdViewFactory m_ViewFactory;
		private readonly ConnectProTheme m_Theme;

		private IConnectProRoom m_Room;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="viewFactory"></param>
		/// <param name="theme"></param>
		public ConnectProOsdNavigationController(IOsdViewFactory viewFactory, ConnectProTheme theme)
		{
			m_Cache = new Dictionary<Type, IOsdPresenter>();
			m_CacheSection = new SafeCriticalSection();

			m_ViewFactory = viewFactory;
			m_Theme = theme;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Updates the presenters to track the given room.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			if (room == m_Room)
				return;

			m_Room = room;

			m_CacheSection.Enter();

			try
			{
				foreach (IOsdPresenter presenter in m_Cache.Values.ToArray())
					presenter.SetRoom(m_Room);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			SetRoom(null);

			m_Cache.Values.ForEach(p => p.Dispose());
			m_Cache.Clear();
		}

		/// <summary>
		/// Instantiates or returns an existing presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IOsdPresenter LazyLoadPresenter(Type type)
		{
			IOsdPresenter output;

			m_CacheSection.Enter();

			try
			{
				if (!m_Cache.ContainsKey(type))
					m_Cache[type] = GetNewPresenter(type);
				output = m_Cache[type];
			}
			finally
			{
				m_CacheSection.Leave();
			}

			return output;
		}

		/// <summary>
		/// Instantiates a new presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IOsdPresenter GetNewPresenter(Type type)
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
				throw new Exception(string.Format("Presenter {0} is not of type {1}", output, type.Name));

			return output;
		}

		#endregion
	}
}
