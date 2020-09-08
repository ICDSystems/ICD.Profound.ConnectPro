using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters
{
	[PresenterBinding(typeof(IGenericKeyboardPresenter))]
	public sealed class GenericKeyboardPresenter : AbstractTouchDisplayPresenter<IGenericKeyboardView>, IGenericKeyboardPresenter
	{
		private readonly KeypadStringBuilder m_StringBuilder;

		private bool m_RefreshTextField;
		private bool m_Shift;
		private bool m_Caps;

		private string m_Prompt;

		#region Properties

		/// <summary>
		/// Gets/sets the prompt.
		/// </summary>
		public string Prompt
		{
			get { return m_Prompt; }
			set
			{
				if (value == m_Prompt)
					return;

				m_Prompt = value;

				RefreshIfVisible();
			}
		}

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

		/// <summary>
		/// Gets/sets the submit callback
		/// </summary>
		private Action<string> SubmitCallback { get; set; }

		/// <summary>
		/// Gets/sets the change callback
		/// </summary>
		private Action<string> ChangeCallback { get; set; }

		/// <summary>
		/// Gets/sets the close callback.
		/// </summary>
		private Action<string> CloseCallback { get; set; }

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public GenericKeyboardPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme) 
			: base(nav, views, theme)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;

			m_RefreshTextField = true;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IGenericKeyboardView view)
		{
			base.Refresh(view);
			
			view.SetPrompt(Prompt);
			view.SelectCapsButton(Caps);
			view.SelectShiftButton(Shift);
			view.SetShift(Shift, Caps);

			if (m_RefreshTextField)
				view.SetText(m_StringBuilder.ToString());
		}

		/// <summary>
		/// Shows the view using the given callback for the enter button.
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="text"></param>
		/// <param name="enterCallback"></param>
		/// <param name="closeCallback"></param>
		/// <param name="changeCallback"></param>
		public void ShowView(string prompt, string text, Action<string> enterCallback, Action<string> closeCallback, Action<string> changeCallback)
		{
			Prompt = prompt;

			m_StringBuilder.Clear();
			m_StringBuilder.SetString(text);

			SubmitCallback = enterCallback;
			CloseCallback = closeCallback;
			ChangeCallback = changeCallback;

			ShowView(true);
		}

		/// <summary>
		/// Called when the stringbuilder string changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void StringBuilderOnStringChanged(object sender, StringEventArgs stringEventArgs)
		{
			if (ChangeCallback != null)
				ChangeCallback(stringEventArgs.Data);

			RefreshIfVisible();
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
		protected override void Subscribe(IGenericKeyboardView view)
		{
			base.Subscribe(view);

			view.OnBackspaceButtonPressed += ViewOnBackspaceButtonPressed;
			view.OnTextEntered += ViewOnTextEntered;
			view.OnCapsButtonPressed += ViewOnCapsButtonPressed;
			view.OnShiftButtonPressed += ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed += ViewOnSpaceButtonPressed;
			view.OnKeyPressed += ViewOnKeyPressed;
			view.OnSubmitButtonPressed += ViewOnSubmitButtonPressed;
			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IGenericKeyboardView view)
		{
			base.Unsubscribe(view);

			view.OnBackspaceButtonPressed -= ViewOnBackspaceButtonPressed;
			view.OnTextEntered -= ViewOnTextEntered;
			view.OnCapsButtonPressed -= ViewOnCapsButtonPressed;
			view.OnShiftButtonPressed -= ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed -= ViewOnSpaceButtonPressed;
			view.OnKeyPressed -= ViewOnKeyPressed;
			view.OnSubmitButtonPressed -= ViewOnSubmitButtonPressed;
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
		private void ViewOnSubmitButtonPressed(object sender, EventArgs eventArgs)
		{
			if (SubmitCallback != null)
				SubmitCallback(m_StringBuilder.ToString());

			ShowView(false);
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
		/// Called when the user presses the backspace button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnBackspaceButtonPressed(object sender, EventArgs eventArgs)
		{
			m_StringBuilder.Backspace();
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
		/// Called when the user presses the caps button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCapsButtonPressed(object sender, EventArgs eventArgs)
		{
			Caps = !Caps;
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			if (CloseCallback != null)
				CloseCallback(m_StringBuilder.ToString());

			ShowView(false);
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

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (args.Data)
				return;
			
			SubmitCallback = null;
			ChangeCallback = null;

			m_StringBuilder.Clear();
		}

		#endregion
	}
}
