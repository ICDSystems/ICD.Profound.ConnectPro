using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu
{
	[PresenterBinding(typeof(IWtcReferencedLeftMenuPresenter))]
	public sealed class WtcReferencedLeftMenuPresenter : AbstractUiComponentPresenter<IWtcReferencedLeftMenuView>, IWtcReferencedLeftMenuPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private IWtcLeftMenuButtonModel m_ButtonModel;

		/// <summary>
		/// Gets/sets the model for this child item.
		/// </summary>
		public IWtcLeftMenuButtonModel ButtonModel
		{
			get { return m_ButtonModel; }
			set
			{
				if (value == m_ButtonModel)
					return;

				Unsubscribe(m_ButtonModel);
				m_ButtonModel = value;
				Subscribe(m_ButtonModel);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
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
				string icon = ButtonModel == null ? string.Empty : ButtonModel.Icon;
				string label = ButtonModel == null ? string.Empty : ButtonModel.Label;
				bool state = ButtonModel != null && ButtonModel.State;
				bool hasState = ButtonModel != null && ButtonModel.HasState;

				view.SetIcon(icon);
				view.SetLabelText(label);
				view.SetStatusLightState(state);
				view.SetStatusLightVisible(hasState);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Model Callbacks

		/// <summary>
		/// Subscribe to the model events.
		/// </summary>
		/// <param name="buttonModel"></param>
		private void Subscribe(IWtcLeftMenuButtonModel buttonModel)
		{
			if (buttonModel == null)
				return;

			buttonModel.OnStateChanged += ButtonOnStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the model events.
		/// </summary>
		/// <param name="buttonModel"></param>
		private void Unsubscribe(IWtcLeftMenuButtonModel buttonModel)
		{
			if (buttonModel == null)
				return;

			buttonModel.OnStateChanged -= ButtonOnStateChanged;
		}

		/// <summary>
		/// Called when the model button state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ButtonOnStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

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
			if (ButtonModel != null)
				ButtonModel.HandlePress();
		}

		#endregion
	}
}