using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.UI.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcKeyboardPresenter : AbstractPresenter<IVtcKeyboardView>, IVtcKeyboardPresenter
	{
		public event EventHandler OnExitButtonPressed; 

		private readonly KeypadStringBuilder m_StringBuilder;

		private bool m_Shift;
		private bool m_Caps;
		private bool m_RefreshTextField;

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
			m_StringBuilder = new KeypadStringBuilder();
			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;

			m_RefreshTextField = true;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnExitButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcKeyboardView view)
		{
			base.Refresh(view);

			if (m_RefreshTextField)
				view.SetText(m_StringBuilder.ToString());

			view.SelectCapsButton(Caps);
			view.SelectShiftButton(Shift);
			view.SetShift(Shift, Caps);
		}

		/// <summary>
		/// Called when the stringbuilder string changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void StringBuilderOnStringChanged(object sender, StringEventArgs stringEventArgs)
		{
			// Refresh synchronously to avoid interfering with user input.
			RefreshIfVisible(false);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcKeyboardView view)
		{
			base.Subscribe(view);

			view.OnBackspaceButtonPressed += ViewOnBackspaceButtonPressed;
			view.OnCapsButtonPressed += ViewOnCapsButtonPressed;
			view.OnSubmitButtonPressed += ViewOnSubmitButtonPressed;
			view.OnShiftButtonPressed += ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed += ViewOnSpaceButtonPressed;
			view.OnTextEntered += ViewOnTextEntered;
			view.OnKeyPressed += ViewOnKeyPressed;
			view.OnExitButtonPressed += ViewOnExitButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcKeyboardView view)
		{
			base.Unsubscribe(view);

			view.OnBackspaceButtonPressed -= ViewOnBackspaceButtonPressed;
			view.OnCapsButtonPressed -= ViewOnCapsButtonPressed;
			view.OnSubmitButtonPressed -= ViewOnSubmitButtonPressed;
			view.OnShiftButtonPressed -= ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed -= ViewOnSpaceButtonPressed;
			view.OnTextEntered -= ViewOnTextEntered;
			view.OnKeyPressed -= ViewOnKeyPressed;
			view.OnExitButtonPressed -= ViewOnExitButtonPressed;
		}

		/// <summary>
		/// Called when the user enters text directly in the text field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void ViewOnTextEntered(object sender, StringEventArgs stringEventArgs)
		{
			m_RefreshTextField = false;
			m_StringBuilder.SetString(stringEventArgs.Data);
			m_RefreshTextField = true;
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
			m_StringBuilder.AppendCharacter(character);
		}

		/// <summary>
		/// Called when the user presses the space bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSpaceButtonPressed(object sender, EventArgs eventArgs)
		{
			m_StringBuilder.AppendCharacter(' ');
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
		/// Called when the user presses the submit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSubmitButtonPressed(object sender, EventArgs eventArgs)
		{
			string number = m_StringBuilder.ToString();

			IConferenceManager manager = Room == null ? null : Room.ConferenceManager;
			if (manager != null)
				manager.Dial(number);

			ShowView(false);
		}

		private void ViewOnExitButtonPressed(object sender, EventArgs eventArgs)
		{
			OnExitButtonPressed.Raise(this);
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
		/// Called when the iser presses the backspace button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnBackspaceButtonPressed(object sender, EventArgs eventArgs)
		{
			m_StringBuilder.Backspace();
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

			m_StringBuilder.Clear();
		}

		#endregion
	}
}
