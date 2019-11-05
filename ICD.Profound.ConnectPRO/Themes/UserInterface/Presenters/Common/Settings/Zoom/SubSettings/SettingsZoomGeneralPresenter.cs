using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.SettingsTree.Zoom;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom.SubSettings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom.SubSettings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Zoom.SubSettings
{
	[PresenterBinding(typeof(ISettingsZoomGeneralPresenter))]
	public sealed class SettingsZoomGeneralPresenter : AbstractSettingsZoomSubPresenter<ISettingsZoomGeneralView>, ISettingsZoomGeneralPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsZoomGeneralPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsZoomGeneralView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool muteAll = Settings != null && Settings.MuteAllParticipantsAtMeetingStart;
				view.SetMuteAllButtonSelected(muteAll);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Callbacks

		/// <summary>
		/// Subscribe to the settings events.
		/// </summary>
		/// <param name="settings"></param>
		protected override void Subscribe(ZoomSettingsLeaf settings)
		{
			base.Subscribe(settings);

			if (settings == null)
				return;

			settings.OnMuteAllParticipantsAtMeetingStartChanged += SettingsOnMuteAllParticipantsAtMeetingStartChanged;
		}

		/// <summary>
		/// Unsubscribe from the settings events.
		/// </summary>
		/// <param name="settings"></param>
		protected override void Unsubscribe(ZoomSettingsLeaf settings)
		{
			base.Unsubscribe(settings);

			if (settings == null)
				return;

			settings.OnMuteAllParticipantsAtMeetingStartChanged -= SettingsOnMuteAllParticipantsAtMeetingStartChanged;
		}

		private void SettingsOnMuteAllParticipantsAtMeetingStartChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsZoomGeneralView view)
		{
			base.Subscribe(view);

			view.OnMuteAllParticipantsButtonPressed += ViewOnMuteAllParticipantsButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsZoomGeneralView view)
		{
			base.Unsubscribe(view);

			view.OnMuteAllParticipantsButtonPressed -= ViewOnMuteAllParticipantsButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the mute all button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnMuteAllParticipantsButtonPressed(object sender, EventArgs e)
		{
			Settings.SetMuteAllParticipantsAtMeetingStart(!Settings.MuteAllParticipantsAtMeetingStart);
		}

		#endregion
	}
}

