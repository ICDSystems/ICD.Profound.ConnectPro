using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Layout;
using ICD.Connect.Conferencing.Controls.Routing;
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

		private static readonly BiDictionary<ushort, Type> s_IndexToPresenterType = new BiDictionary<ushort, Type>
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

		public CameraButtonsPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_CameraActivePresenter = Navigation.LazyLoadPresenter<ICameraActivePresenter>();
			m_CameraControlPresenter = Navigation.LazyLoadPresenter<ICameraControlPresenter>();
			m_CameraLayoutPresenter = Navigation.LazyLoadPresenter<ICameraLayoutPresenter>();
		}

		protected override void Refresh(ICameraButtonsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetCameraConfigurationButtonEnabled(0, CameraActiveEnabled());
				view.SetCameraConfigurationButtonEnabled(1, CameraControlEnabled());
				view.SetCameraConfigurationButtonEnabled(2, true);

				foreach (ushort index in s_IndexToPresenterType.Keys.Where(k => k != m_Index))
				{
					view.SetCameraConfigurationButtonSelected(index, false);
				}

				view.SetCameraConfigurationButtonSelected(m_Index, true);
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
			m_CameraLayoutPresenter.SetDestinationControl(value == null
				                                              ? null
				                                              : value.Parent.Controls
				                                                     .GetControl<IConferenceLayoutControl>());

		}

		#region Private Methods

		private bool CameraActiveEnabled()
		{
			return m_CameraActivePresenter.CameraCount > 1;
		}

		private bool CameraControlEnabled()
		{
			return m_CameraControlPresenter.CameraCount > 0;
		}

		/// <summary>
		/// Navigates to the child sub-page based on the given index.
		/// </summary>
		/// <param name="index"></param>
		private void NavigateTo(ushort index)
		{
			m_Index = index;

			foreach (Type type in s_IndexToPresenterType.Where(kvp => kvp.Key != m_Index).Select(kvp => kvp.Value))
				Navigation.LazyLoadPresenter(type).ShowView(false);

			Navigation.LazyLoadPresenter(s_IndexToPresenterType.GetValue(m_Index)).ShowView(true);

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(ICameraButtonsView view)
		{
			base.Subscribe(view);

			view.OnCameraConfigurationButtonPressed += ViewOnCameraConfigurationButtonPressed;
		}

		protected override void Unsubscribe(ICameraButtonsView view)
		{
			base.Unsubscribe(view);

			view.OnCameraConfigurationButtonPressed -= ViewOnCameraConfigurationButtonPressed;
		}

		private void ViewOnCameraConfigurationButtonPressed(object sender, UShortEventArgs e)
		{
			switch (e.Data)
			{
				case 0:
					NavigateTo(0);
					break;
				case 1:
					NavigateTo(1);
					break;
				case 2:
					NavigateTo(2);
					break;

				default:
					throw new ArgumentOutOfRangeException("e");
			}

			RefreshIfVisible();
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

			// When the view becomes visible show the child presenter based on enable state.
			if (args.Data)
			{
				if (CameraActiveEnabled())
					m_Index = 0;
				else if (CameraControlEnabled())
					m_Index = 1;
				else
					m_Index = 2;

				NavigateTo(m_Index);
			}
		}

		#endregion
	}
}
