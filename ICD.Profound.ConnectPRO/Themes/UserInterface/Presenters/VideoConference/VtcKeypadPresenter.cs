using System;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.EventArguments;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	[PresenterBinding(typeof(IVtcKeypadPresenter))]
	public sealed class VtcKeypadPresenter : AbstractVtcBaseKeyboardPresenter<IVtcKeypadView>, IVtcKeypadPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcKeypadPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcKeypadView view)
		{
			base.Subscribe(view);

			view.OnKeyPressed += ViewOnKeyPressed;
			view.OnKeyboardButtonPressed += ViewOnKeyboardButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcKeypadView view)
		{
			base.Unsubscribe(view);

			view.OnKeyPressed -= ViewOnKeyPressed;
			view.OnKeyboardButtonPressed -= ViewOnKeyboardButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnKeyPressed(object sender, KeyboardKeyEventArgs eventArgs)
		{
			char character = eventArgs.Data.GetChar(false, false);

			AppendCharacter(character);
		}

		/// <summary>
		/// Called when the user presses the keyboard button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnKeyboardButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.LazyLoadPresenter<IVtcKeyboardPresenter>().ShowView(DialCallback);

			ShowView(false);
		}

		#endregion
	}
}