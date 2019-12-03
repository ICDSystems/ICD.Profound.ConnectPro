using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Layout;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Cameras
{
	[PresenterBinding(typeof(ICameraButtonsPresenter))]
	public sealed class CameraButtonsPresenter : AbstractPopupPresenter<ICameraButtonsView>, ICameraButtonsPresenter
	{
		private sealed class CameraButton
		{
			public string Name { get; set; }
			public IUiPresenter Presenter { get; set; }
			public Func<bool> VisibleCallback { get; set; }

			public bool Selected { get { return Presenter != null && Presenter.IsViewVisible; } }
			public bool Visible { get { return VisibleCallback != null && VisibleCallback(); } }
		}

		private readonly List<CameraButton> m_Buttons;
		private readonly ICameraControlPresenter m_CameraControlPresenter;
		private readonly ICameraLayoutPresenter m_CameraLayoutPresenter;
		private readonly SafeCriticalSection m_RefreshSection;

		#region Properties

		private bool CameraControlVisible { get { return m_CameraControlPresenter.CameraCount > 0; } }

		private bool CameraLayoutVisible { get; set; }

		/// <summary>
		/// Returns true if any of the child features (layout, etc) are currently available.
		/// </summary>
		public bool AnyFeaturesAvailable { get { return CameraControlVisible || CameraLayoutVisible; } }

		#endregion

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

			m_CameraControlPresenter = Navigation.LazyLoadPresenter<ICameraControlPresenter>();
			m_CameraLayoutPresenter = Navigation.LazyLoadPresenter<ICameraLayoutPresenter>();

			m_Buttons = new List<CameraButton>
			{
				new CameraButton
				{
					Name = "Control Camera",
					VisibleCallback = () => CameraControlVisible,
					Presenter = m_CameraControlPresenter
				},
				new CameraButton
				{
					Name = "Camera Layout",
					VisibleCallback = () => CameraLayoutVisible,
					Presenter = m_CameraLayoutPresenter
				}
			};
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
				view.SetButtons(m_Buttons.Select(b => b.Name));

				for (ushort index = 0; index < m_Buttons.Count; index++)
				{
					CameraButton button = m_Buttons[index];

					view.SetButtonSelected(index, button.Selected);
					view.SetButtonVisible(index, button.Visible);
				}
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
			m_CameraLayoutPresenter.SetConferenceLayoutControl(value == null
				                                                   ? null
				                                                   : value.Parent.Controls
				                                                          .GetControl<IConferenceLayoutControl
				                                                          >());

			CameraLayoutVisible = value is ZoomRoomConferenceControl;
		}

		#region Private Methods

		/// <summary>
		/// Navigates to the child sub-page based on the given index.
		/// </summary>
		/// <param name="index"></param>
		private void NavigateTo(ushort index)
		{
			CameraButton button;
			if (!m_Buttons.TryElementAt(index, out button))
				throw new ArgumentOutOfRangeException("index");

			NavigateTo(button.Presenter);
		}

		private void NavigateTo(IUiPresenter presenter)
		{
			// First hide the other presenters
			foreach (CameraButton button in m_Buttons.Where(b => b.Presenter != presenter))
				button.Presenter.ShowView(false);

			presenter.ShowView(true);

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

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICameraButtonsView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		private void ViewOnButtonPressed(object sender, UShortEventArgs e)
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

			// Hide the children before hiding the parent.
			foreach (CameraButton button in m_Buttons)
				button.Presenter.ShowView(false);
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
			if (CameraControlVisible)
				NavigateTo(m_CameraControlPresenter);
			else if (CameraLayoutVisible)
				NavigateTo(m_CameraLayoutPresenter);
			else
				ShowView(false);
		}

		#endregion
	}
}
