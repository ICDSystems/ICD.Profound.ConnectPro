using System;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	public abstract class AbstractWtcReferencedLeftMenuPresenter : AbstractUiComponentPresenter<IWtcReferencedLeftMenuView>,
	                                                               IWtcReferencedLeftMenuPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private IWebConferenceDeviceControl m_ConferenceControl;

		#region Properties

		/// <summary>
		/// Gets/sets the active conference control for this presenter.
		/// </summary>
		public IWebConferenceDeviceControl ActiveConferenceControl
		{
			get { return m_ConferenceControl; }
			set
			{
				if (m_ConferenceControl == value)
					return;

				Unsubscribe(m_ConferenceControl);
				m_ConferenceControl = value;
				Subscribe(m_ConferenceControl);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets the label for the button.
		/// </summary>
		protected string Label { get; set; }

		/// <summary>
		/// Gets/sets the icon for the button.
		/// </summary>
		protected string Icon { get; set; }

		/// <summary>
		/// Gets/sets the feedback state for the light.
		/// </summary>
		protected bool? State { get; set; }

		/// <summary>
		/// Gets/sets the selected state for the button.
		/// </summary>
		protected bool Selected { get; set; }

		/// <summary>
		/// Gets/sets the enabled state for the entire subpage.
		/// </summary>
		protected bool Enabled { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                              ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedLeftMenuView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string icon = Icon ?? string.Empty;
				string label = Label ?? string.Empty;
				bool state = State == true;
				bool hasState = State != null;

				icon = Icons.GetIcon(icon, eIconColor.White);

				view.SetIcon(icon);
				view.SetLabelText(label);
				view.SetStatusLightState(state);
				view.SetStatusLightVisible(hasState);
				view.SetButtonSelected(Selected);

				view.Enable(Enabled);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Hides the child subpage/s managed by this button.
		/// </summary>
		public abstract void HideSubpages();

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IWtcReferencedLeftMenuView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWtcReferencedLeftMenuView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Enabled)
				HandleButtonPress();
		}

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected abstract void HandleButtonPress();

		#endregion

		#region Conference Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected virtual void Subscribe(IWebConferenceDeviceControl control)
		{
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected virtual void Unsubscribe(IWebConferenceDeviceControl control)
		{
		}

		#endregion
	}
}
