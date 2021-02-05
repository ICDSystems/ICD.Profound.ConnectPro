using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;
using ICD.Profound.ConnectPROCommon.SettingsTree.Zoom;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Zoom
{
	[PresenterBinding(typeof(ISettingsZoomSpeakersPresenter))]
	public sealed class SettingsZoomSpeakersPresenter :
		AbstractSettingsNodeBasePresenter<ISettingsZoomSpeakersView, ZoomSpeakerSettingsLeaf>,
		ISettingsZoomSpeakersPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private string m_SelectedSpeakerName;
		private AudioOutputLine[] m_Speakers;

		public string SelectedSpeakerName
		{
			get { return m_SelectedSpeakerName; }
			private set
			{
				if (value == m_SelectedSpeakerName)
					return;

				m_SelectedSpeakerName = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsZoomSpeakersPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                     IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_Speakers = new AudioOutputLine[0];
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsZoomSpeakersView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				// Update labels
				view.SetSpeakerLabels(m_Speakers.Select(m => GetSpeakerLabel(m)));

				// Set speaker selection state.
				for (ushort index = 0; index < m_Speakers.Length; index++)
					view.SetSpeakerButtonSelected(index, m_SelectedSpeakerName == m_Speakers[index].ShortName);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private static string GetSpeakerLabel([NotNull] AudioOutputLine speaker)
		{
			if (speaker == null)
				throw new ArgumentNullException("speaker");

			return string.Format("{0} ({1})", speaker.ShortName, speaker.Id);
		}

		private void UpdateSpeakers()
		{
			m_Speakers = Node == null ? new AudioOutputLine[0] : Node.GetSpeakers().ToArray();
			RefreshIfVisible();
		}

		private void UpdateSelectedSpeaker()
		{
			SelectedSpeakerName = Node == null ? null : Node.SelectedSpeakerName;
		}

		#endregion

		#region Settings Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(ZoomSpeakerSettingsLeaf node)
		{
			base.Subscribe(node);

			if (node == null)
				return;

			node.OnSelectedSpeakerNameChanged += SettingsOnSelectedSpeakerNameChanged;
			node.OnSpeakersChanged += SettingsOnSpeakersChanged;

			UpdateSpeakers();
			UpdateSelectedSpeaker();
		}

		/// <summary>
		/// Unsubscribe from the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(ZoomSpeakerSettingsLeaf node)
		{
			base.Unsubscribe(node);

			if (node == null)
				return;

			node.OnSelectedSpeakerNameChanged -= SettingsOnSelectedSpeakerNameChanged;
			node.OnSpeakersChanged -= SettingsOnSpeakersChanged;

			UpdateSpeakers();
			UpdateSelectedSpeaker();
		}

		/// <summary>
		/// Called when the selected speaker changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SettingsOnSelectedSpeakerNameChanged(object sender, EventArgs eventArgs)
		{
			UpdateSelectedSpeaker();
		}

		/// <summary>
		/// Called when the available speakers cahnge.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SettingsOnSpeakersChanged(object sender, EventArgs eventArgs)
		{
			UpdateSpeakers();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsZoomSpeakersView view)
		{
			base.Subscribe(view);

			view.OnSpeakerButtonPressed += ViewOnSpeakerButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsZoomSpeakersView view)
		{
			base.Unsubscribe(view);

			view.OnSpeakerButtonPressed -= ViewOnSpeakerButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a speaker button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnSpeakerButtonPressed(object sender, UShortEventArgs e)
		{
			AudioOutputLine speaker;
			if (!m_Speakers.TryElementAt(e.Data, out speaker))
				return;

			Node.SetSelectedSpeaker(speaker.ShortName);
		}

		#endregion
	}
}
