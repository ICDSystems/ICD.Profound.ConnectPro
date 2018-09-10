using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcKeyboardPresenter : AbstractVtcBaseKeyboardPresenter<IVtcKeyboardView>, IVtcKeyboardPresenter
	{
		private bool m_Shift;
		private bool m_Caps;

		#region Properties

		/// <summary>
		/// Gets/sets the caps state.
		/// </summary>
		public bool Caps
		{
			get { return m_Caps; }
			set
			{
				if (value == m_Caps)
					return;

				m_Caps = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets the shift state.
		/// </summary>
		public bool Shift
		{
			get { return m_Shift; }
			set
			{
				if (value == m_Shift)
					return;

				m_Shift = value;

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
		public VtcKeyboardPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcKeyboardView view)
		{
			base.Refresh(view);

			view.SelectCapsButton(Caps);
			view.SelectShiftButton(Shift);
			view.SetShift(Shift, Caps);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcKeyboardView view)
		{
			base.Subscribe(view);

			view.OnCapsButtonPressed += ViewOnCapsButtonPressed;
			view.OnShiftButtonPressed += ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed += ViewOnSpaceButtonPressed;
			view.OnKeyPressed += ViewOnKeyPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcKeyboardView view)
		{
			base.Unsubscribe(view);

			view.OnCapsButtonPressed -= ViewOnCapsButtonPressed;
			view.OnShiftButtonPressed -= ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed -= ViewOnSpaceButtonPressed;
			view.OnKeyPressed -= ViewOnKeyPressed;
			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnKeyPressed(object sender, KeyboardKeyEventArgs eventArgs)
		{
			char character = eventArgs.Data.GetChar(Shift, Caps);

			Shift = false;
			AppendCharacter(character);
		}

		/// <summary>
		/// Called when the user presses the space bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSpaceButtonPressed(object sender, EventArgs eventArgs)
		{
			AppendCharacter(' ');
		}

		/// <summary>
		/// Called when the user presses the shift button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnShiftButtonPressed(object sender, EventArgs eventArgs)
		{
			Shift = !Shift;
		}

		/// <summary>
		/// Called when the user presses the keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnKeypadButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);

			Navigation.LazyLoadPresenter<IVtcKeypadPresenter>().ShowView(DialCallback);
		}

		/// <summary>
		/// Called when the user presses the caps button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCapsButtonPressed(object sender, EventArgs eventArgs)
		{
			Caps = !Caps;
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			Caps = false;
			Shift = false;
		}

		#endregion
	}
}
