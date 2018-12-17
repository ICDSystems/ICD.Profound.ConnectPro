using System;
using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	/// <summary>
	/// Provides a way for presenters to access their views.
	/// </summary>
	public sealed class ConnectProUiViewFactory : AbstractViewFactory, IUiViewFactory
	{
		private delegate IUiView FactoryMethod(ISigInputOutput panel, ConnectProTheme theme);

		private delegate IUiView ComponentFactoryMethod(
			ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index);

		private readonly Dictionary<Type, ComponentFactoryMethod> m_ComponentViewFactories = new Dictionary
			<Type, ComponentFactoryMethod>
		{
			// Popups
			{typeof(IReferencedCableTvView), (nav, views, theme, index) => new ReferencedCableTvView(nav, views, theme, index)},
			{typeof(IReferencedWebConferencingAlertView), (nav, views, theme, index) => new ReferencedWebConferencingAlertView(nav, views, theme, index)},

			// Sources
			{typeof(IReferencedSourceSelectView), (nav, views, theme, index) => new ReferencedSourceSelectView(nav, views, theme, index)},

			// Video Conference
			{typeof(IVtcReferencedContactsView), (nav, views, theme, index) => new VtcReferencedContactsView(nav, views, theme, index)},
			{typeof(IVtcReferencedDtmfView), (nav, views, theme, index) => new VtcReferencedDtmfView(nav, views, theme, index)},
			{typeof(IVtcReferencedActiveCallsView), (nav, views, theme, index) => new VtcReferencedActiveCallsView(nav, views, theme, index)},

			// Web Conference
			{typeof(IWtcReferencedDirectoryItemView), (nav, views, theme, index) => new WtcReferencedDirectoryItemView(nav, views, theme, index)},
			{typeof(IWtcReferencedParticipantView), (nav, views, theme, index) => new WtcReferencedParticipantView(nav, views, theme, index)},

			// Common
			{typeof(IReferencedScheduleView), (nav, views, theme, index) => new ReferencedScheduleView(nav, views, theme, index)}
		};

		private readonly Dictionary<Type, FactoryMethod> m_ViewFactories = new Dictionary<Type, FactoryMethod>
		{
			// Popups
			{typeof(IAppleTvView), (panel, theme) => new AppleTvView(panel, theme)},
			{typeof(ICableTvView), (panel, theme) => new CableTvView(panel, theme)},
			{typeof(IWebConferencingAlertView), (panel, theme) => new WebConferencingAlertView(panel, theme)},
			{typeof(IWebConferencingStepView), (panel, theme) => new WebConferencingStepView(panel, theme)},

			// Displays
			{typeof(IMenuDisplaysView), (panel, theme) => new MenuDisplaysView(panel, theme)},

			// Options
			{typeof(IOptionPrivacyMuteView), (panel, theme) => new OptionPrivacyMuteView(panel, theme)},
			{typeof(IOptionVolumeView), (panel, theme) => new OptionVolumeView(panel, theme)},
			{typeof(IOptionCameraView), (panel, theme) => new OptionCameraView(panel, theme)},

			// Sources
			{typeof(ISourceSelectView), (panel, theme) => new SourceSelectView(panel, theme)},

			// Common
			{typeof(IConfirmEndMeetingView), (panel, theme) => new ConfirmEndMeetingView(panel, theme)},
			{typeof(IConfirmLeaveCallView), (panel, theme) => new ConfirmLeaveCallView(panel, theme)},
			{typeof(IConfirmSplashPowerView), (panel, theme) => new ConfirmSplashPowerView(panel, theme)},
			{typeof(IEndMeetingView), (panel, theme) => new EndMeetingView(panel, theme)},
			{typeof(IHeaderView), (panel, theme) => new HeaderView(panel, theme)},
			{typeof(IStartMeetingView), (panel, theme) => new StartMeetingView(panel, theme)},
			{typeof(IVolumeView), (panel, theme) => new VolumeView(panel, theme)},
			{typeof(IDisabledAlertView), (panel, theme) => new DisabledAlertView(panel, theme)},
			{typeof(IPasscodeView), (panel, theme) => new PasscodeView(panel, theme)},
			{typeof(IGenericAlertView), (panel, theme) => new GenericAlertView(panel, theme)},

			// Settings
			{typeof(ISettingsBaseView), (panel, theme) => new SettingsBaseView(panel, theme)},
			{typeof(ISettingsSystemPowerView), (panel, theme) => new SettingsSystemPowerView(panel, theme)},
			{typeof(ISettingsPasscodeView), (panel, theme) => new SettingsPasscodeView(panel, theme)},
			{typeof(ISettingsDirectoryView), (panel, theme) => new SettingsDirectoryView(panel, theme)},

			// Video Conference
			{typeof(IVtcBaseView), (panel, theme) => new VtcBaseView(panel, theme)},
			{typeof(IVtcContactsNormalView), (panel, theme) => new VtcContactsNormalView(panel, theme)},
			{typeof(IVtcContactsPolycomView), (panel, theme) => new VtcContactsPolycomView(panel, theme)},
			{typeof(ICameraControlView), (panel, theme) => new CameraControlView(panel, theme)},
			{typeof(IVtcShareView), (panel, theme) => new VtcShareView(panel, theme)},
			{typeof(IVtcDtmfView), (panel, theme) => new VtcDtmfView(panel, theme)},
			{typeof(IVtcIncomingCallView), (panel, theme) => new VtcIncomingCallView(panel, theme)},
			{typeof(IVtcActiveCallsView), (panel, theme) => new VtcActiveCallsView(panel, theme)},
			{typeof(IVtcCallListToggleView), (panel, theme) => new VtcCallListToggleView(panel, theme)},
			{typeof(IVtcButtonListView), (panel, theme) => new VtcButtonListView(panel, theme)},
			{typeof(IVtcKeyboardView), (panel, theme) => new VtcKeyboardView(panel, theme)},
			{typeof(IVtcKeypadView), (panel, theme) => new VtcKeypadView(panel, theme)},

			// Web Conference
			{typeof(IWtcBaseView), (panel, theme) => new WtcBaseView(panel, theme)},
			{typeof(IWtcButtonListView), (panel, theme) => new WtcButtonListView(panel, theme)},
			{typeof(IWtcStartMeetingView), (panel, theme) => new WtcStartMeetingView(panel, theme)},
			{typeof(IWtcContactListView), (panel, theme) => new WtcContactListView(panel, theme)},
			{typeof(IWtcActiveMeetingView), (panel, theme) => new WtcActiveMeetingView(panel, theme)},
			{typeof(IWtcRecordingView), (panel, theme) => new WtcRecordingView(panel, theme)},
			{typeof(IWtcShareView), (panel, theme) => new WtcShareView(panel, theme)},
			{typeof(IWtcCallOutView), (panel, theme) => new WtcCallOutView(panel, theme)},
			{typeof(IWtcActiveMeetingToggleView), (panel, theme) => new WtcActiveMeetingToggleView(panel, theme)},
			{typeof(IWtcContactsToggleView), (panel, theme) => new WtcContactsToggleView(panel, theme)},

			// Audio Conference
			{typeof(IAtcBaseView), (panel, theme) => new AtcBaseView(panel, theme)},
			{typeof(IAtcIncomingCallView), (panel, theme) => new AtcIncomingCallView(panel, theme)},

			// Panel
			{typeof(IHardButtonsView), (panel, theme) => new HardButtonsView(panel, theme)}
		};

		private readonly ConnectProTheme m_Theme;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProUiViewFactory(IPanelDevice panel, ConnectProTheme theme)
			: base(panel)
		{
			m_Theme = theme;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Instantiates a new view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected override T InstantiateView<T>()
		{
			if (!m_ViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			FactoryMethod factory = m_ViewFactories[typeof(T)];
			IUiView output = factory(Panel, m_Theme);

			if (output as T == null)
				throw new InvalidCastException(string.Format("View {0} is not of type {1}", output, typeof(T).Name));

			return output as T;
		}

		/// <summary>
		/// Instantiates a view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override T InstantiateView<T>(ISmartObject panel, IVtProParent parent, ushort index)
		{
			if (!m_ComponentViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			ComponentFactoryMethod factory = m_ComponentViewFactories[typeof(T)];
			IUiView output = factory(panel, m_Theme, parent, index);

			if (output as T == null)
				throw new InvalidCastException(string.Format("View {0} is not of type {1}", output, typeof(T).Name));

			return output as T;
		}

		#endregion
	}
}
