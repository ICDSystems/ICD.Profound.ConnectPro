using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Layout;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Cameras
{
	[PresenterBinding(typeof(ICameraButtonsPresenter))]
	public sealed class CameraButtonsPresenter : AbstractUiPresenter<ICameraButtonsView>, ICameraButtonsPresenter
	{
		private const ushort CAMERA_ACTIVE_INDEX = 0;
		private const ushort CAMERA_CONTROL_INDEX = 1;
		private const ushort CAMERA_LAYOUT_INDEX = 2;

		private static readonly BiDictionary<ushort, Type> s_IndexToPresenterType =
			new BiDictionary<ushort, Type>
			{
				{CAMERA_ACTIVE_INDEX, typeof(ICameraActivePresenter)},
				{CAMERA_CONTROL_INDEX, typeof(ICameraControlPresenter)},
				{CAMERA_LAYOUT_INDEX, typeof(ICameraLayoutPresenter)}
			};

		private readonly ICameraActivePresenter m_CameraActivePresenter;
		private readonly ICameraControlPresenter m_CameraControlPresenter;
		private readonly ICameraLayoutPresenter m_CameraLayoutPresenter;
		private readonly SafeCriticalSection m_RefreshSection;

		private ushort m_Index;
		private bool m_ZoomMode;

		private bool CameraActiveEnabled { get { return m_CameraActivePresenter.CameraCount > 1; } }

		private bool CameraControlEnabled { get { return m_CameraControlPresenter.CameraCount > 0; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CameraButtonsPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_CameraActivePresenter = Navigation.LazyLoadPresenter<ICameraActivePresenter>();
			m_CameraControlPresenter = Navigation.LazyLoadPresenter<ICameraControlPresenter>();
			m_CameraLayoutPresenter = Navigation.LazyLoadPresenter<ICameraLayoutPresenter>();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ICameraButtonsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				// Button enabled states
				view.SetCameraConfigurationButtonEnabled(CAMERA_ACTIVE_INDEX, CameraActiveEnabled);
				view.SetCameraConfigurationButtonEnabled(CAMERA_CONTROL_INDEX, CameraControlEnabled);
				view.SetCameraConfigurationButtonEnabled(CAMERA_LAYOUT_INDEX, m_ZoomMode);

				// Button selection states
				foreach (ushort index in s_IndexToPresenterType.Keys)
					view.SetCameraConfigurationButtonSelected(index, index == m_Index);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the conference control that is currently active.
		/// </summary>
		/// <param name="value"></param>
		public void SetActiveConferenceControl(IConferenceDeviceControl value)
		{
			m_CameraActivePresenter.SetVtcDestinationControl(value == null
				                                                 ? null
				                                                 : value.Parent.Controls
				                                                        .GetControl<IVideoConferenceRouteControl>());

			m_CameraLayoutPresenter.SetConferenceLayoutControl(value == null
				                                                    ? null
				                                                    : value.Parent.Controls
				                                                           .GetControl<IConferenceLayoutControl
				                                                           >());

			m_ZoomMode = value is ZoomRoomConferenceControl;
		}

		#region Private Methods

		/// <summary>
		/// Navigates to the child sub-page based on the given index.
		/// </summary>
		/// <param name="index"></param>
		private void NavigateTo(ushort index)
		{
			if (!s_IndexToPresenterType.ContainsKey(index))
				throw new ArgumentOutOfRangeException("index");

			m_Index = index;

			// Subpage visibility
			foreach (KeyValuePair<ushort, Type> kvp in s_IndexToPresenterType)
				Navigation.LazyLoadPresenter(kvp.Value).ShowView(kvp.Key == m_Index);

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICameraButtonsView view)
		{
			base.Subscribe(view);

			view.OnCameraConfigurationButtonPressed += ViewOnCameraConfigurationButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICameraButtonsView view)
		{
			base.Unsubscribe(view);

			view.OnCameraConfigurationButtonPressed -= ViewOnCameraConfigurationButtonPressed;
		}

		private void ViewOnCameraConfigurationButtonPressed(object sender, UShortEventArgs e)
		{
			NavigateTo(e.Data);
		}

		/// <summary>
		/// Called when the view visibility is about to change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			if (args.Data)
				return;

			//Hide the children before hiding the parent.
			foreach (Type presenterType in s_IndexToPresenterType.Values)
				Navigation.LazyLoadPresenter(presenterType).ShowView(false);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (!args.Data)
				return;

			// When the view becomes visible show the child presenter based on enable state.
			if (CameraControlEnabled)
				NavigateTo(CAMERA_CONTROL_INDEX);
			else if (CameraActiveEnabled)
				NavigateTo(CAMERA_ACTIVE_INDEX);
			else if(m_ZoomMode)
				NavigateTo(CAMERA_LAYOUT_INDEX);
			else
				ShowView(false);
		}

		#endregion
	}
}
