using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Responses;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;
using ICD.Profound.ConnectPROCommon.SettingsTree.Zoom;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Zoom
{
	[PresenterBinding(typeof (ISettingsZoomMicrophonesPresenter))]
	public sealed class SettingsZoomMicrophonesPresenter :
		AbstractSettingsNodeBasePresenter<ISettingsZoomMicrophonesView, ZoomMicrophoneSettingsLeaf>,
		ISettingsZoomMicrophonesPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private string m_SelectedMicrophoneName;
		private AudioInputLine[] m_Microphones;

		public string SelectedMicrophoneName
		{
			get { return m_SelectedMicrophoneName; }
			private set
			{
				if (value == m_SelectedMicrophoneName)
					return;

				m_SelectedMicrophoneName = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsZoomMicrophonesPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                        IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_Microphones = new AudioInputLine[0];
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsZoomMicrophonesView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				// Update labels
				view.SetMicrophoneLabels(m_Microphones.Select(m => GetMicrophoneLabel(m)));

				// Set microphone selection state.
				for (ushort index = 0; index < m_Microphones.Length; index++)
					view.SetMicrophoneButtonSelected(index, SelectedMicrophoneName == m_Microphones[index].ShortName);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private static string GetMicrophoneLabel([NotNull] AudioInputLine microphone)
		{
			if (microphone == null)
				throw new ArgumentNullException("microphone");

			return string.Format("{0} ({1})", microphone.ShortName, microphone.Id);
		}

		private void UpdateMicrophones()
		{
			m_Microphones = Node == null ? new AudioInputLine[0] : Node.GetMicrophones().ToArray();
			RefreshIfVisible();
		}

		private void UpdateSelectedMicrophone()
		{
			SelectedMicrophoneName = Node == null ? null : Node.SelectedMicrophoneName;
		}

		#endregion

		#region Settings Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(ZoomMicrophoneSettingsLeaf node)
		{
			base.Subscribe(node);

			if (node == null)
				return;

			node.OnSelectedMicrophoneNameChanged += SettingsOnSelectedMicrophoneNameChanged;
			node.OnMicrophonesChanged += SettingsOnMicrophonesChanged;

			UpdateMicrophones();
			UpdateSelectedMicrophone();
		}

		/// <summary>
		/// Unsubscribe from the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(ZoomMicrophoneSettingsLeaf node)
		{
			base.Unsubscribe(node);

			if (node == null)
				return;

			node.OnSelectedMicrophoneNameChanged -= SettingsOnSelectedMicrophoneNameChanged;
			node.OnMicrophonesChanged -= SettingsOnMicrophonesChanged;

			UpdateMicrophones();
			UpdateSelectedMicrophone();
		}

		/// <summary>
		/// Called when the selected microphone changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SettingsOnSelectedMicrophoneNameChanged(object sender, EventArgs eventArgs)
		{
			UpdateSelectedMicrophone();
		}

		/// <summary>
		/// Called when the available microphones cahnge.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SettingsOnMicrophonesChanged(object sender, EventArgs eventArgs)
		{
			UpdateMicrophones();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsZoomMicrophonesView view)
		{
			base.Subscribe(view);

			view.OnMicrophoneButtonPressed += ViewOnMicrophoneButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsZoomMicrophonesView view)
		{
			base.Unsubscribe(view);

			view.OnMicrophoneButtonPressed -= ViewOnMicrophoneButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a microphone button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnMicrophoneButtonPressed(object sender, UShortEventArgs e)
		{
			AudioInputLine microphone;
			if (!m_Microphones.TryElementAt(e.Data, out microphone))
				return;

			Node.SetSelectedMicrophone(microphone.ShortName);
		}

		#endregion
	}
}
