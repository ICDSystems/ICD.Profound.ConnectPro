using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	[PresenterBinding(typeof(IWtcKeyboardPresenter))]
	public sealed class WtcKeyboardPresenter : AbstractUiPresenter<IWtcKeyboardView>, IWtcKeyboardPresenter
	{
		public event EventHandler<StringEventArgs> OnStringChanged;
		public event EventHandler<StringEventArgs> OnEnterPressed;
		public event EventHandler OnClosePressed;

		private readonly KeypadStringBuilder m_StringBuilder;

		private bool m_RefreshTextField;
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
		public WtcKeyboardPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;

			m_RefreshTextField = true;
		}

		public override void Dispose()
		{
			base.Dispose();

			m_StringBuilder.OnStringChanged -= StringBuilderOnStringChanged;

			OnStringChanged = null;
			OnClosePressed = null;
			OnEnterPressed = null;
		}

		public void Show(string text)
		{
			m_StringBuilder.SetString(text);
			ShowView(true);
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

			OnStringChanged.Raise(this, new StringEventArgs(m_StringBuilder.ToString()));
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcKeyboardView view)
		{
			base.Refresh(view);

			if (m_RefreshTextField)
				view.SetText(m_StringBuilder.ToString());

			view.SelectCapsButton(Caps);
			view.SelectShiftButton(Shift);
			view.SetShift(Shift, Caps);
		}

		/// <summary>
		/// Appends the given character to the builder.
		/// </summary>
		/// <param name="character"></param>
		private void AppendCharacter(char character)
		{
			m_StringBuilder.AppendCharacter(character);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IWtcKeyboardView view)
		{
			base.Subscribe(view);

			view.OnBackspaceButtonPressed += ViewOnBackspaceButtonPressed;
			view.OnTextEntered += ViewOnTextEntered;
			view.OnEnterButtonPressed += ViewOnEnterButtonPressed;
			view.OnCapsButtonPressed += ViewOnCapsButtonPressed;
			view.OnShiftButtonPressed += ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed += ViewOnSpaceButtonPressed;
			view.OnKeyPressed += ViewOnKeyPressed;
			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWtcKeyboardView view)
		{
			base.Unsubscribe(view);

			view.OnBackspaceButtonPressed -= ViewOnBackspaceButtonPressed;
			view.OnTextEntered -= ViewOnTextEntered;
			view.OnEnterButtonPressed -= ViewOnEnterButtonPressed;
			view.OnCapsButtonPressed -= ViewOnCapsButtonPressed;
			view.OnShiftButtonPressed -= ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed -= ViewOnSpaceButtonPressed;
			view.OnKeyPressed -= ViewOnKeyPressed;
			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
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
		/// Called when the user presses the submit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnEnterButtonPressed(object sender, EventArgs eventArgs)
		{
			string searchString = m_StringBuilder.ToString();
			ShowView(false);
			
			OnEnterPressed.Raise(this, new StringEventArgs(searchString));
		}

		/// <summary>
		/// Called when the user presses the backspace button.
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
			
			if (!args.Data)
				m_StringBuilder.Clear();
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
		private void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);

			OnClosePressed.Raise(this);
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
		/// Called when the view visibility is about to change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			if (args.Data)
			{
				Caps = false;
				Shift = false;
			}
		}

		#endregion
	}
}
