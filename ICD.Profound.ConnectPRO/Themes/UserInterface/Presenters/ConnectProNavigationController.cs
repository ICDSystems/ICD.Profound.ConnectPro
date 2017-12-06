#if !SIMPLSHARP
using System.Reflection;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	/// <summary>
	/// Provides a way for presenters to access each other.
	/// </summary>
	public sealed class ConnectProNavigationController : INavigationController
	{
		private delegate IPresenter PresenterFactory(INavigationController nav, IViewFactory views, ConnectProTheme theme);

		private readonly Dictionary<Type, PresenterFactory> m_PresenterFactories = new Dictionary<Type, PresenterFactory>
		{
			// Popups
			{typeof(IAppleTvPresenter), (nav, views, theme) => new AppleTvPresenter(nav, views, theme)},

			// Displays
			{typeof(IDisplaysPresenter), (nav, views, theme) => new DisplaysPresenter(nav, views, theme)},
			{typeof(IReferencedDisplaysPresenter), (nav, views, theme) => new ReferencedDisplaysPresenter(nav, views, theme)},

			// Options
			{typeof(IOptionContactsPresenter), (nav, views, theme) => new OptionContactsPresenter(nav, views, theme)},
			{typeof(IOptionMasterMicPresenter), (nav, views, theme) => new OptionMasterMicPresenter(nav, views, theme)},
			{typeof(IOptionPrivacyMutePresenter), (nav, views, theme) => new OptionPrivacyMutePresenter(nav, views, theme)},
			{typeof(IOptionVolumePresenter), (nav, views, theme) => new OptionVolumePresenter(nav, views, theme)},

			// Sources
			{typeof(ISourceSelectDualPresenter), (nav, views, theme) => new SourceSelectDualPresenter(nav, views, theme)},
			{typeof(ISourceSelectSinglePresenter), (nav, views, theme) => new SourceSelectSinglePresenter(nav, views, theme)},
			{typeof(IReferencedSourceSelectPresenter), (nav, views, theme) => new ReferencedSourceSelectPresenter(nav, views, theme)},

			// Common
			{typeof(IEndMeetingPresenter), (nav, views, theme) => new EndMeetingPresenter(nav, views, theme)},
			{typeof(IHeaderPresenter), (nav, views, theme) => new HeaderPresenter(nav, views, theme)},
			{typeof(IStartMeetingPresenter), (nav, views, theme) => new StartMeetingPresenter(nav, views, theme)}
		};

		private readonly Dictionary<Type, IPresenter> m_Cache;
		private readonly SafeCriticalSection m_CacheSection;
		private readonly IViewFactory m_ViewFactory;
		private readonly ConnectProTheme m_Theme;

		private IRoom m_Room;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="viewFactory"></param>
		/// <param name="theme"></param>
		public ConnectProNavigationController(IViewFactory viewFactory, ConnectProTheme theme)
		{
			m_Cache = new Dictionary<Type, IPresenter>();
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
		public void SetRoom(IRoom room)
		{
			if (room == m_Room)
				return;

			m_Room = room;

			m_CacheSection.Enter();

			try
			{
				foreach (IPresenter presenter in m_Cache.Values.ToArray())
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
		public IPresenter LazyLoadPresenter(Type type)
		{
			IPresenter output;

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
		public IPresenter GetNewPresenter(Type type)
		{
			if (!m_PresenterFactories.ContainsKey(type))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, type.Name);
				throw new KeyNotFoundException(message);
			}

			PresenterFactory factory = m_PresenterFactories[type];
			IPresenter output = factory(this, m_ViewFactory, m_Theme);
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
