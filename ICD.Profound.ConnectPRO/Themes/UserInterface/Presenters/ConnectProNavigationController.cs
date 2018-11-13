#if !SIMPLSHARP
using System.Reflection;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts;

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
			{typeof(ICableTvPresenter), (nav, views, theme) => new CableTvPresenter(nav, views, theme)},
			{typeof(IReferencedCableTvPresenter), (nav, views, theme) => new ReferencedCableTvPresenter(nav, views, theme)},
			{typeof(IWebConferencingAlertPresenter), (nav, views, theme) => new WebConferencingAlertPresenter(nav, views, theme)},
			{typeof(IReferencedWebConferencingAlertPresenter), (nav, views, theme) => new ReferencedWebConferencingAlertPresenter(nav, views, theme)},
			{typeof(IWebConferencingStepPresenter), (nav, views, theme) => new WebConferencingStepPresenter(nav, views, theme)},

			// Displays
			{typeof(IMenuDisplaysPresenter), (nav, views, theme) => new MenuDisplaysPresenter(nav, views, theme)},

			// Options
			{typeof(IOptionPrivacyMutePresenter), (nav, views, theme) => new OptionPrivacyMutePresenter(nav, views, theme)},
			{typeof(IOptionVolumePresenter), (nav, views, theme) => new OptionVolumePresenter(nav, views, theme)},
			{typeof(IOptionCameraPresenter), (nav, views, theme) => new OptionCameraPresenter(nav, views, theme)},

			// Sources
			{typeof(ISourceSelectPresenter), (nav, views, theme) => new SourceSelectPresenter(nav, views, theme)},
			{typeof(IReferencedSourceSelectPresenter), (nav, views, theme) => new ReferencedSourceSelectPresenter(nav, views, theme)},

			// Common
			{typeof(IConfirmEndMeetingPresenter), (nav, views, theme) => new ConfirmEndMeetingPresenter(nav, views, theme)},
			{typeof(IConfirmLeaveCallPresenter), (nav, views, theme) => new ConfirmLeaveCallPresenter(nav, views, theme)},
			{typeof(IConfirmSplashPowerPresenter), (nav, views, theme) => new ConfirmSplashPowerPresenter(nav, views, theme)},
			{typeof(IEndMeetingPresenter), (nav, views, theme) => new EndMeetingPresenter(nav, views, theme)},
			{typeof(IHeaderPresenter), (nav, views, theme) => new HeaderPresenter(nav, views, theme)},
			{typeof(IStartMeetingPresenter), (nav, views, theme) => new StartMeetingPresenter(nav, views, theme)},
			{typeof(IReferencedSchedulePresenter), (nav, views, theme) => new ReferencedSchedulePresenter(nav, views, theme)},
			{typeof(IVolumePresenter), (nav, views, theme) => new VolumePresenter(nav, views, theme)},
			{typeof(IDisabledAlertPresenter), (nav, views, theme) => new DisabledAlertPresenter(nav, views, theme)},
			{typeof(IPasscodePresenter), (nav, views, theme) => new PasscodePresenter(nav, views, theme)},

			// Settings
			{typeof(ISettingsBasePresenter), (nav, views, theme) => new SettingsBasePresenter(nav, views, theme)},
			{typeof(ISettingsSystemPowerPresenter), (nav, views, theme) => new SettingsSystemPowerPresenter(nav, views, theme)},
			{typeof(ISettingsPasscodePresenter), (nav, views, theme) => new SettingsPasscodePresenter(nav, views, theme)},
			{typeof(ISettingsDirectoryPresenter), (nav, views, theme) => new SettingsDirectoryPresenter(nav, views, theme)},

			// Video Conference
			{typeof(IVtcBasePresenter), (nav, views, theme) => new VtcBasePresenter(nav, views, theme)},
			{typeof(IVtcContactsNormalPresenter), (nav, views, theme) => new VtcContactsNormalPresenter(nav, views, theme)},
			{typeof(IVtcContactsPolycomPresenter), (nav, views, theme) => new VtcContactsPolycomPresenter(nav, views, theme)},
			{typeof(IVtcCameraPresenter), (nav, views, theme) => new VtcCameraPresenter(nav, views, theme)},
			{typeof(IVtcSharePresenter), (nav, views, theme) => new VtcSharePresenter(nav, views, theme)},
			{typeof(IVtcDtmfPresenter), (nav, views, theme) => new VtcDtmfPresenter(nav, views, theme)},
			{typeof(IVtcReferencedDtmfPresenter), (nav, views, theme) => new VtcReferencedDtmfPresenter(nav, views, theme)},
			{typeof(IVtcIncomingCallPresenter), (nav, views, theme) => new VtcIncomingCallPresenter(nav, views, theme)},
			{typeof(IVtcActiveCallsPresenter), (nav, views, theme) => new VtcActiveCallsPresenter(nav, views, theme)},
			{typeof(IVtcReferencedActiveCallsPresenter), (nav, views, theme) => new VtcReferencedActiveCallsPresenter(nav, views, theme)},
			{typeof(IVtcCallListTogglePresenter), (nav, views, theme) => new VtcCallListTogglePresenter(nav, views, theme)},
			{typeof(IVtcButtonListPresenter), (nav, views, theme) => new VtcButtonListPresenter(nav, views, theme)},
			{typeof(IVtcKeyboardPresenter), (nav, views, theme) => new VtcKeyboardPresenter(nav, views, theme)},
			{typeof(IVtcKeypadPresenter), (nav, views, theme) => new VtcKeypadPresenter(nav, views, theme)},

			// Web Conference
			{typeof(IWtcBasePresenter), (nav, views, theme) => new WtcBasePresenter(nav, views, theme)},
			{typeof(IWtcMainPagePresenter), (nav, views, theme) => new WtcMainPagePresenter(nav, views, theme)},
			{typeof(IWtcContactListPresenter), (nav, views, theme) => new WtcContactListPresenter(nav, views, theme)},
			{typeof(IWtcReferencedContactPresenter), (nav, views, theme) => new WtcReferencedContactPresenter(nav, views, theme)},
			{typeof(IWtcJoinByIdPresenter), (nav, views, theme) => new WtcJoinByIdPresenter(nav, views, theme)},
			{typeof(IWtcButtonListPresenter), (nav, views, theme) => new WtcButtonListPresenter(nav, views, theme)},
			{typeof(IWtcActiveMeetingPresenter), (nav, views, theme) => new WtcActiveMeetingPresenter(nav, views, theme)},
			{typeof(IWtcReferencedParticipantPresenter), (nav, views, theme) => new WtcReferencedParticipantPresenter(nav, views, theme)},
			{typeof(IWtcSharePresenter), (nav, views, theme) => new WtcSharePresenter(nav, views, theme)},
			{typeof(IWtcRecordingPresenter), (nav, views, theme) => new WtcRecordingPresenter(nav, views, theme)},
			{typeof(IWtcCallOutPresenter), (nav, views, theme) => new WtcCallOutPresenter(nav, views, theme)},
			{typeof(IWtcActiveMeetingTogglePresenter), (nav, views, theme) => new WtcActiveMeetingTogglePresenter(nav, views, theme)},
			{typeof(IWtcContactsTogglePresenter), (nav, views, theme) => new WtcContactsTogglePresenter(nav, views, theme)},

			// Video Conference Contacts
			{typeof(IVtcReferencedContactsPresenter), (nav, views, theme) => new VtcReferencedContactsPresenter(nav, views, theme)},
			{typeof(IVtcReferencedFavoritesPresenter), (nav, views, theme) => new VtcReferencedFavoritesPresenter(nav, views, theme)},
			{typeof(IVtcReferencedRecentPresenter), (nav, views, theme) => new VtcReferencedRecentPresenter(nav, views, theme)},
			{typeof(IVtcReferencedFolderPresenter), (nav, views, theme) => new VtcReferencedFolderPresenter(nav, views, theme)},

			// Audio Conference
			{typeof(IAtcBasePresenter), (nav, views, theme) => new AtcBasePresenter(nav, views, theme)},
			{typeof(IAtcIncomingCallPresenter), (nav, views, theme) => new AtcIncomingCallPresenter(nav, views, theme)},

			// Panel
			{typeof(IHardButtonsPresenter), (nav, views, theme) => new HardButtonsPresenter(nav, views, theme)}
		};

		private readonly Dictionary<Type, IPresenter> m_Cache;
		private readonly SafeCriticalSection m_CacheSection;
		private readonly IViewFactory m_ViewFactory;
		private readonly ConnectProTheme m_Theme;

		private IConnectProRoom m_Room;

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
		public void SetRoom(IConnectProRoom room)
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
			if (type == null)
				throw new ArgumentNullException("type");

			m_CacheSection.Enter();

			try
			{
				IPresenter presenter;

				if (!m_Cache.TryGetValue(type, out presenter))
				{
					presenter = GetNewPresenter(type);
					m_Cache[type] = presenter;
				}

				return presenter;
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Instantiates or returns an existing presenter for every presenter that can be assigned to the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IEnumerable<IPresenter> LazyLoadPresenters(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			List<IPresenter> presenters = new List<IPresenter>();

			m_CacheSection.Enter();
			try
			{
				foreach (var presenterType in m_PresenterFactories.Keys)
				{
					if (presenterType.IsAssignableTo(type))
						presenters.Add(LazyLoadPresenter(presenterType));
				}
			}
			finally
			{
				m_CacheSection.Leave();
			}

			return presenters;
		}

		/// <summary>
		/// Instantiates a new presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IPresenter GetNewPresenter(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			PresenterFactory factory;

			if (!m_PresenterFactories.TryGetValue(type, out factory))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, type.Name);
				throw new KeyNotFoundException(message);
			}

			IPresenter output = factory(this, m_ViewFactory, m_Theme);
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
