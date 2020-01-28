using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Layout;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.Conferencing.Zoom.Components.Layout;
using ICD.Connect.Conferencing.Zoom.Controls;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.Camera;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.Camera;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Conference.Camera
{
	[PresenterBinding(typeof(ICameraLayoutPresenter))]
	public sealed class CameraLayoutPresenter : AbstractTouchDisplayPresenter<ICameraLayoutView>, ICameraLayoutPresenter
	{
		private static readonly BiDictionary<ushort, eZoomLayoutStyle> s_LayoutStyles =
			new BiDictionary<ushort, eZoomLayoutStyle>
			{
				{0, eZoomLayoutStyle.Gallery},
				{1, eZoomLayoutStyle.Speaker},
				{2, eZoomLayoutStyle.Strip},
				{3, eZoomLayoutStyle.ShareAll}
			};

		private static readonly BiDictionary<ushort, eZoomLayoutSize> s_LayoutSizes =
			new BiDictionary<ushort, eZoomLayoutSize>
			{
				{0, eZoomLayoutSize.Off},
				{1, eZoomLayoutSize.Size1},
				{2, eZoomLayoutSize.Size2},
				{3, eZoomLayoutSize.Size3}
			};

		private static readonly BiDictionary<ushort, eZoomLayoutPosition> s_LayoutPositions =
			new BiDictionary<ushort, eZoomLayoutPosition>
			{
				{0, eZoomLayoutPosition.UpLeft},
				{1, eZoomLayoutPosition.UpRight},
				{2, eZoomLayoutPosition.DownLeft},
				{3, eZoomLayoutPosition.DownRight}
			};

		private readonly SafeCriticalSection m_RefreshSection;

		[CanBeNull]
		private IConferenceLayoutControl m_ConferenceLayoutControl;

		[CanBeNull]
		private LayoutComponent m_LayoutComponent;

		[CanBeNull]
		private IPresentationControl m_PresentationControl;

		[CanBeNull]
		private CallComponent m_CallComponent;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CameraLayoutPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme) 
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Sets the wrapped conference layout control.
		/// </summary>
		/// <param name="control"></param>
		public void SetConferenceLayoutControl(IConferenceLayoutControl control)
		{
			if (control == m_ConferenceLayoutControl)
				return;

			Unsubscribe(m_ConferenceLayoutControl);
			m_ConferenceLayoutControl = control;
			Subscribe(m_ConferenceLayoutControl);

			LayoutComponent layoutComponent = GetZoomLayoutComponent(control);
			SetLayoutComponent(layoutComponent);

			IPresentationControl presentationControl = GetPresentationControl(control);
			SetPresentationControl(presentationControl);

			CallComponent callComponent = GetZoomCallComponent(control);
			SetCallComponent(callComponent);

			RefreshIfVisible();
		}

		protected override void Refresh(ICameraLayoutView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				// Set the selected state of the controls
				eZoomLayoutStyle style =
					m_LayoutComponent == null
						? eZoomLayoutStyle.None
						: m_LayoutComponent.LayoutStyle;
				eZoomLayoutSize size =
					m_LayoutComponent == null
						? eZoomLayoutSize.None
						: m_LayoutComponent.LayoutSize;
				eZoomLayoutPosition position =
					m_LayoutComponent == null
						? eZoomLayoutPosition.None
						: m_LayoutComponent.LayoutPosition;
				bool selfView = m_LayoutComponent != null && m_LayoutComponent.SelfViewEnabled;
				bool contentThumbnail = m_LayoutComponent != null && m_LayoutComponent.ShareThumb;


				// Set the enabled state of the controls
				bool layoutAvailable = m_ConferenceLayoutControl != null && m_ConferenceLayoutControl.LayoutAvailable;
				bool presentationActive = m_PresentationControl != null && m_PresentationControl.PresentationActive;
				int participantCount = m_CallComponent == null ? 0 : m_CallComponent.GetParticipants().Count();

				bool sizeEnabled = m_LayoutComponent != null && (layoutAvailable && m_LayoutComponent.LayoutAvailability.CanAdjustFloatingVideo);
				bool styleEnabled = layoutAvailable;
				bool contentThumbnailEnabled = m_LayoutComponent != null && (layoutAvailable && presentationActive &&
				                                                             m_LayoutComponent.LayoutAvailability.CanSwitchFloatingShareContent);
				bool selfviewCameraEnabled = layoutAvailable && participantCount > 1;
				bool thumbnailPositionEnabled = m_LayoutComponent != null &&
				                                (layoutAvailable && size != eZoomLayoutSize.Off && m_LayoutComponent
				                                                                                   .LayoutAvailability
				                                                                                   .CanAdjustFloatingVideo
				                                );

				bool galleryEnabled = layoutAvailable && m_LayoutComponent.LayoutAvailability.CanSwitchWallView;
				bool speakerEnabled = layoutAvailable && m_LayoutComponent.LayoutAvailability.CanSwitchSpeakerView;
				bool stripEnabled = layoutAvailable;
				bool shareAllEnabled =
					layoutAvailable && m_LayoutComponent.LayoutAvailability.CanSwitchShareOnAllScreens;

				view.SetLayoutSizeListEnabled(sizeEnabled);
				view.SetLayoutStyleListEnabled(styleEnabled);
				view.SetContentThumbnailButtonEnabled(contentThumbnailEnabled);
				view.SetSelfviewCameraButtonEnabled(selfviewCameraEnabled);

				view.SetLayoutStyleGalleryItemEnabled(galleryEnabled);
				view.SetLayoutStyleSpeakerItemEnabled(speakerEnabled);
				view.SetLayoutStyleStripItemEnabled(stripEnabled);
				view.SetLayoutStyleShareAllItemEnabled(shareAllEnabled);
				
				view.SetSelfviewCameraButtonSelected(selfView);
				view.SetContentThumbnailButtonSelected(contentThumbnail);
				foreach (KeyValuePair<ushort, eZoomLayoutPosition> kvp in s_LayoutPositions)
					view.SetThumbnailPositionButtonVisibility(kvp.Key, thumbnailPositionEnabled && kvp.Value != position);
				foreach (KeyValuePair<ushort, eZoomLayoutSize> kvp in s_LayoutSizes)
					view.SetLayoutSizeButtonSelected(kvp.Key, kvp.Value == size);
				foreach (KeyValuePair<ushort, eZoomLayoutStyle> kvp in s_LayoutStyles)
					view.SetLayoutStyleButtonSelected(kvp.Key, kvp.Value == style);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		/// <summary>
		/// Sets the wrapped presentation control.
		/// </summary>
		/// <param name="presentationControl"></param>
		private void SetPresentationControl(IPresentationControl presentationControl)
		{
			if (presentationControl == m_PresentationControl)
				return;

			Unsubscribe(m_PresentationControl);
			m_PresentationControl = presentationControl;
			Subscribe(m_PresentationControl);

			RefreshIfVisible();
		}

		/// <summary>
		/// Sets the wrapped Zoom layout component.
		/// </summary>
		/// <param name="component"></param>
		private void SetLayoutComponent(LayoutComponent component)
		{
			if (component == m_LayoutComponent)
				return;

			Unsubscribe(m_LayoutComponent);
			m_LayoutComponent = component;
			Subscribe(m_LayoutComponent);

			RefreshIfVisible();
		}

		/// <summary>
		/// Sets the wrapped Zoom call component.
		/// </summary>
		/// <param name="component"></param>
		private void SetCallComponent(CallComponent component)
		{
			if (component == m_CallComponent)
				return;

			Unsubscribe(m_CallComponent);
			m_CallComponent = component;
			Subscribe(m_CallComponent);

			RefreshIfVisible();
		}

		/// <summary>
		/// Gets the zoom layout component for the given layout control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		[CanBeNull]
		private static LayoutComponent GetZoomLayoutComponent([CanBeNull] IConferenceLayoutControl control)
		{
			ZoomRoomLayoutControl zoomLayoutControl = control as ZoomRoomLayoutControl;
			return zoomLayoutControl == null
				       ? null
				       : zoomLayoutControl.Parent.Components.GetComponent<LayoutComponent>();
		}

		/// <summary>
		/// Gets the zoom call component for the given layout control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		[CanBeNull]
		private static CallComponent GetZoomCallComponent([CanBeNull] IConferenceLayoutControl control)
		{
			ZoomRoomLayoutControl zoomLayoutControl = control as ZoomRoomLayoutControl;
			return zoomLayoutControl == null
				       ? null
				       : zoomLayoutControl.Parent.Components.GetComponent<CallComponent>();
		}

		/// <summary>
		/// Gets the presentation control for the given layout control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		[CanBeNull]
		private static IPresentationControl GetPresentationControl([CanBeNull] IConferenceLayoutControl control)
		{
			return control == null ? null : control.Parent.Controls.GetControl<IPresentationControl>();
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribe to the conference layout control events.
		/// </summary>
		/// <param name="control"></param>
		private void Subscribe(IConferenceLayoutControl control)
		{
			if (control == null)
				return;

			control.OnLayoutAvailableChanged += ControlOnLayoutAvailableChanged;
			control.OnSelfViewEnabledChanged += ControlOnSelfViewEnabledChanged;
			control.OnSelfViewFullScreenEnabledChanged += ControlOnSelfViewFullScreenEnabledChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference layout control events.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IConferenceLayoutControl control)
		{
			if (control == null)
				return;

			control.OnLayoutAvailableChanged -= ControlOnLayoutAvailableChanged;
			control.OnSelfViewEnabledChanged -= ControlOnSelfViewEnabledChanged;
			control.OnSelfViewFullScreenEnabledChanged -= ControlOnSelfViewFullScreenEnabledChanged;
		}

		/// <summary>
		/// Called when the fullscreen enabled state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ControlOnSelfViewFullScreenEnabledChanged(object sender, ConferenceLayoutSelfViewFullScreenApiEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the selfview enabled state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ControlOnSelfViewEnabledChanged(object sender, ConferenceLayoutSelfViewApiEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when layout availability changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ControlOnLayoutAvailableChanged(object sender, ConferenceLayoutAvailableApiEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region LayoutComponent Callbacks

		/// <summary>
		/// Subscribe to the layout component events.
		/// </summary>
		/// <param name="layoutComponent"></param>
		private void Subscribe(LayoutComponent layoutComponent)
		{
			if (layoutComponent == null)
				return;

			layoutComponent.OnPositionChanged += LayoutComponentOnPositionChanged;
			layoutComponent.OnShareThumbChanged += LayoutComponentOnShareThumbChanged;
			layoutComponent.OnSizeChanged += LayoutComponentOnSizeChanged;
			layoutComponent.OnStyleChanged += LayoutComponentOnStyleChanged;
			layoutComponent.OnCallLayoutAvailabilityChanged += LayoutComponentOnCallLayoutAvailabilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the layout component events.
		/// </summary>
		/// <param name="layoutComponent"></param>
		private void Unsubscribe(LayoutComponent layoutComponent)
		{
			if (layoutComponent == null)
				return;

			layoutComponent.OnPositionChanged -= LayoutComponentOnPositionChanged;
			layoutComponent.OnShareThumbChanged -= LayoutComponentOnShareThumbChanged;
			layoutComponent.OnSizeChanged -= LayoutComponentOnSizeChanged;
			layoutComponent.OnStyleChanged -= LayoutComponentOnStyleChanged;
			layoutComponent.OnCallLayoutAvailabilityChanged -= LayoutComponentOnCallLayoutAvailabilityChanged;
		}

		/// <summary>
		/// Called when the layout style changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LayoutComponentOnStyleChanged(object sender, ZoomLayoutStyleEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the layout size changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LayoutComponentOnSizeChanged(object sender, ZoomLayoutSizeEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the layout share thumbnail changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LayoutComponentOnShareThumbChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the layout position changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LayoutComponentOnPositionChanged(object sender, ZoomLayoutPositionEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when then availability of layout features changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LayoutComponentOnCallLayoutAvailabilityChanged(object sender, GenericEventArgs<ZoomLayoutAvailability> e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region CallComponent Callbacks

		/// <summary>
		/// Subscribe to the call component events.
		/// </summary>
		/// <param name="callComponent"></param>
		private void Subscribe(CallComponent callComponent)
		{
			if (callComponent == null)
				return;

			callComponent.OnParticipantAdded += CallComponentOnParticipantAdded;
			callComponent.OnParticipantRemoved += CallComponentOnParticipantRemoved;
		}

		/// <summary>
		/// Unsubscribe from the call component events.
		/// </summary>
		/// <param name="callComponent"></param>
		private void Unsubscribe(CallComponent callComponent)
		{
			if (callComponent == null)
				return;

			callComponent.OnParticipantAdded -= CallComponentOnParticipantAdded;
			callComponent.OnParticipantRemoved -= CallComponentOnParticipantRemoved;
		}

		/// <summary>
		/// Called when a participant is removed from the active conference.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="genericEventArgs"></param>
		private void CallComponentOnParticipantRemoved(object sender, GenericEventArgs<ParticipantInfo> genericEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when a participant is added to the active conference.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="genericEventArgs"></param>
		private void CallComponentOnParticipantAdded(object sender, GenericEventArgs<ParticipantInfo> genericEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region PresentationControl Callbacks

		/// <summary>
		/// Subscribe to the presentation control events.
		/// </summary>
		/// <param name="presentationControl"></param>
		private void Subscribe(IPresentationControl presentationControl)
		{
			if (presentationControl == null)
				return;

			presentationControl.OnPresentationActiveChanged += PresentationControlOnPresentationActiveChanged;
		}

		/// <summary>
		/// Unsubscribe from the presentation control events.
		/// </summary>
		/// <param name="presentationControl"></param>
		private void Unsubscribe(IPresentationControl presentationControl)
		{
			if (presentationControl == null)
				return;

			presentationControl.OnPresentationActiveChanged -= PresentationControlOnPresentationActiveChanged;
		}

		/// <summary>
		/// Called when the presentation active state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="presentationActiveApiEventArgs"></param>
		private void PresentationControlOnPresentationActiveChanged(object sender, PresentationActiveApiEventArgs presentationActiveApiEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICameraLayoutView view)
		{
			base.Subscribe(view);
			
			view.OnLayoutSizeButtonPressed += ViewOnLayoutSizeButtonPressed;
			view.OnLayoutStyleButtonPressed += ViewOnLayoutStyleButtonPressed;
			view.OnContentThumbnailButtonPressed += ViewOnContentThumbnailButtonPressed;
			view.OnSelfviewCameraButtonPressed += ViewOnSelfviewCameraButtonPressed;
			view.OnThumbnailPositionButtonPressed += ViewOnThumbnailPositionButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICameraLayoutView view)
		{
			base.Unsubscribe(view);
			
			view.OnLayoutSizeButtonPressed -= ViewOnLayoutSizeButtonPressed;
			view.OnLayoutStyleButtonPressed -= ViewOnLayoutStyleButtonPressed;
			view.OnContentThumbnailButtonPressed -= ViewOnContentThumbnailButtonPressed;
			view.OnSelfviewCameraButtonPressed -= ViewOnSelfviewCameraButtonPressed;
			view.OnThumbnailPositionButtonPressed -= ViewOnThumbnailPositionButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a layout size button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnLayoutSizeButtonPressed(object sender, UShortEventArgs e)
		{
			var control = m_ConferenceLayoutControl as ZoomRoomLayoutControl;
			if (control == null)
				return;

			eZoomLayoutSize size;
			if (s_LayoutSizes.TryGetValue(e.Data, out size))
				control.SetLayoutSize(size);
		}

		/// <summary>
		/// Called when the user presses a layout style button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnLayoutStyleButtonPressed(object sender, UShortEventArgs e)
		{
			var control = m_ConferenceLayoutControl as ZoomRoomLayoutControl;
			if (control == null)
				return;

			eZoomLayoutStyle style;
			if (s_LayoutStyles.TryGetValue(e.Data, out style))
				control.SetStyle(style);
		}

		/// <summary>
		/// Called when the user presses the content thumbnail button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnContentThumbnailButtonPressed(object sender, EventArgs eventArgs)
		{
			var control = m_ConferenceLayoutControl as ZoomRoomLayoutControl;
			if (control == null)
				return;

			control.SetShareThumbnailEnabled(!control.ShareThumbnailEnabled);
		}

		/// <summary>
		/// Called when the user presses the selfview camera button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSelfviewCameraButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ConferenceLayoutControl != null)
				m_ConferenceLayoutControl.SetSelfViewEnabled(!m_ConferenceLayoutControl.SelfViewEnabled);
		}

		/// <summary>
		/// Called when the user presses a thumbnail position button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnThumbnailPositionButtonPressed(object sender, UShortEventArgs e)
		{
			var control = m_ConferenceLayoutControl as ZoomRoomLayoutControl;
			if (control == null)
				return;

			eZoomLayoutPosition position;
			if (s_LayoutPositions.TryGetValue(e.Data, out position))
				control.SetLayoutPosition(position);
		}

		#endregion
	}
}
