using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;
using ICD.Profound.ConnectPROCommon.SettingsTree.Zoom;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Zoom
{
	[PresenterBinding(typeof(ISettingsZoomGeneralPresenter))]
	public sealed class SettingsZoomGeneralPresenter : AbstractSettingsNodeBasePresenter<ISettingsZoomGeneralView, ZoomGeneralSettingsLeaf>, ISettingsZoomGeneralPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsZoomGeneralPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
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
				bool muteAll = Node != null && Node.MuteAllParticipantsAtMeetingStart;
				bool muteMyCamera = Node != null && Node.MuteMyCameraAtMeetingStart;
				bool enableDialOut = Node != null && Node.EnableDialOut;
				bool enableRecording = Node != null && Node.EnableRecording;

				view.SetMuteAllButtonSelected(muteAll);
				view.SetMuteMyCameraButtonSelected(muteMyCamera);
				view.SetDialOutEnableSelected(enableDialOut);
				view.SetRecordingEnableSelected(enableRecording);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(ZoomGeneralSettingsLeaf node)
		{
			base.Subscribe(node);

			if (node == null)
				return;

			node.OnMuteAllParticipantsAtMeetingStartChanged += SettingsOnMuteAllParticipantsAtMeetingStartChanged;
			node.OnMuteMyCameraAtMeetingStartChanged += SettingsOnMuteMyCameraAtMeetingStartChanged;
			node.OnEnableRecordingChanged += SettingsOnEnableRecordingChanged;
			node.OnEnableDialOutChanged += SettingsOnEnableDialOutChanged;
		}

		/// <summary>
		/// Unsubscribe from the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(ZoomGeneralSettingsLeaf node)
		{
			base.Unsubscribe(node);

			if (node == null)
				return;

			node.OnMuteAllParticipantsAtMeetingStartChanged -= SettingsOnMuteAllParticipantsAtMeetingStartChanged;
			node.OnMuteMyCameraAtMeetingStartChanged -= SettingsOnMuteMyCameraAtMeetingStartChanged;
			node.OnEnableRecordingChanged -= SettingsOnEnableRecordingChanged;
			node.OnEnableDialOutChanged -= SettingsOnEnableDialOutChanged;
		}

		private void SettingsOnMuteAllParticipantsAtMeetingStartChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		private void SettingsOnMuteMyCameraAtMeetingStartChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		private void SettingsOnEnableRecordingChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		private void SettingsOnEnableDialOutChanged(object sender, BoolEventArgs boolEventArgs)
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
			view.OnMuteMyCameraButtonPressed += ViewOnMuteMyCameraButtonPressed;
			view.OnEnableRecordButtonPressed += ViewOnEnableRecordButtonPressed;
			view.OnEnableDialOutButtonPressed += ViewOnEnableDialOutButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsZoomGeneralView view)
		{
			base.Unsubscribe(view);

			view.OnMuteAllParticipantsButtonPressed -= ViewOnMuteAllParticipantsButtonPressed;
			view.OnMuteMyCameraButtonPressed -= ViewOnMuteMyCameraButtonPressed;
			view.OnEnableRecordButtonPressed -= ViewOnEnableRecordButtonPressed;
			view.OnEnableDialOutButtonPressed -= ViewOnEnableDialOutButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the mute all button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnMuteAllParticipantsButtonPressed(object sender, EventArgs e)
		{
			Node.SetMuteAllParticipantsAtMeetingStart(!Node.MuteAllParticipantsAtMeetingStart);
		}

		private void ViewOnMuteMyCameraButtonPressed(object sender, EventArgs eventArgs)
		{
			Node.SetMuteMyCameraAtMeetingStart(!Node.MuteMyCameraAtMeetingStart);
		}

		private void ViewOnEnableRecordButtonPressed(object sender, EventArgs eventArgs)
		{
			Node.SetEnableRecording(!Node.EnableRecording);
		}

		private void ViewOnEnableDialOutButtonPressed(object sender, EventArgs eventArgs)
		{
			Node.SetEnableDialOut(!Node.EnableDialOut);
		}

		#endregion
	}
}

