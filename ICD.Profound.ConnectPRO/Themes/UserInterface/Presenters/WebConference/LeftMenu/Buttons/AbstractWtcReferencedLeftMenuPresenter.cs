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

		private string m_Label;
		private string m_Icon;
		private bool? m_State;
		private bool m_Selected;
		private bool m_Enabled;

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
		/// Gets the label for the button.
		/// </summary>
		protected string Label
		{
			get { return m_Label; }
			set
			{
				if (value == m_Label)
					return;

				m_Label = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets the icon for the button.
		/// </summary>
		protected string Icon
		{
			get { return m_Icon; }
			set
			{
				if (value == m_Label)
					return;

				m_Icon = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets the feedback state for the light.
		/// </summary>
		protected bool? State
		{
			get { return m_State; }
			set
			{
				if (value == m_State)
					return;

				m_State = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Returns true if the button should be selected.
		/// </summary>
		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Returns true if the button should be enabled.
		/// </summary>
		protected bool Enabled
		{
			get { return m_Enabled; }
			set
			{
				if (value == m_Enabled)
					return;

				m_Enabled = value;

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public AbstractWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views,
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
		/// Unsusbcribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected virtual void Unsubscribe(IWebConferenceDeviceControl control)
		{
		}

		#endregion
	}
}
