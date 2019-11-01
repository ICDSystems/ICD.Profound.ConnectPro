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
	[PresenterBinding(typeof(ISettingsZoomAdvancedPresenter))]
	public sealed class SettingsZoomAdvancedPresenter : AbstractSettingsZoomSubPresenter<ISettingsZoomAdvancedView>, ISettingsZoomAdvancedPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsZoomAdvancedPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsZoomAdvancedView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool audioProcessing = Settings != null && Settings.AudioProcessing;
				bool audioReverb = Settings != null && Settings.ReduceAudioReverb;

				view.SetAudioProcessingButtonSelected(audioProcessing);
				view.SetAudioReverbButtonSelected(audioReverb);
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

			settings.OnAudioProcessingChanged += SettingsOnAudioProcessingChanged;
			settings.OnReduceAudioReverbChanged += SettingsOnReduceAudioReverbChanged;
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

			settings.OnAudioProcessingChanged -= SettingsOnAudioProcessingChanged;
			settings.OnReduceAudioReverbChanged -= SettingsOnReduceAudioReverbChanged;
		}

		private void SettingsOnReduceAudioReverbChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		private void SettingsOnAudioProcessingChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
