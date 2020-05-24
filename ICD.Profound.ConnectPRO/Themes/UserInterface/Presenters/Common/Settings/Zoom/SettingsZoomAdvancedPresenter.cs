using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;
using ICD.Profound.ConnectPROCommon.SettingsTree.Zoom;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Zoom
{
	[PresenterBinding(typeof(ISettingsZoomAdvancedPresenter))]
	public sealed class SettingsZoomAdvancedPresenter : AbstractSettingsNodeBasePresenter<ISettingsZoomAdvancedView, ZoomAdvancedSettingsLeaf>, ISettingsZoomAdvancedPresenter
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
				bool audioProcessing = Node != null && Node.AudioProcessing;
				bool audioReverb = Node != null && Node.ReduceAudioReverb;

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
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(ZoomAdvancedSettingsLeaf node)
		{
			base.Subscribe(node);

			if (node == null)
				return;

			node.OnAudioProcessingChanged += SettingsOnAudioProcessingChanged;
			node.OnReduceAudioReverbChanged += SettingsOnReduceAudioReverbChanged;
		}

		/// <summary>
		/// Unsubscribe from the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(ZoomAdvancedSettingsLeaf node)
		{
			base.Unsubscribe(node);

			if (node == null)
				return;

			node.OnAudioProcessingChanged -= SettingsOnAudioProcessingChanged;
			node.OnReduceAudioReverbChanged -= SettingsOnReduceAudioReverbChanged;
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

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsZoomAdvancedView view)
		{
			base.Subscribe(view);

			view.OnAudioProcessingButtonPressed += ViewOnAudioProcessingButtonPressed;
			view.OnAudioReverbButtonPressed += ViewOnAudioReverbButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsZoomAdvancedView view)
		{
			base.Unsubscribe(view);

			view.OnAudioProcessingButtonPressed -= ViewOnAudioProcessingButtonPressed;
			view.OnAudioReverbButtonPressed -= ViewOnAudioReverbButtonPressed;
		}

		private void ViewOnAudioReverbButtonPressed(object sender, EventArgs e)
		{
			Node.SetReduceAudioReverb(!Node.ReduceAudioReverb);
		}

		private void ViewOnAudioProcessingButtonPressed(object sender, EventArgs e)
		{
			Node.SetAudioProcessing(!Node.AudioProcessing);
		}

		#endregion
	}
}
