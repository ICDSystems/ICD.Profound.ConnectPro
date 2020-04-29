using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters
{
	[PresenterBinding(typeof(IGenericAlertPresenter))]
	public sealed class GenericAlertPresenter : AbstractTouchDisplayPresenter<IGenericAlertView>, IGenericAlertPresenter
	{
		private readonly SafeTimer m_CloseTimer;
		private readonly List<GenericAlertPresenterButton> m_Buttons;
		private readonly SafeCriticalSection m_RefreshSection;

		private string m_Message;
		private bool m_Timed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public GenericAlertPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Buttons = new List<GenericAlertPresenterButton>();
			m_CloseTimer = SafeTimer.Stopped(() => ShowView(false));
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IGenericAlertView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetAlertText(m_Message);


				view.SetButtons(m_Buttons.Select(b => b.Label));

				for (ushort index = 0; index < m_Buttons.Count; index++)
				{
					GenericAlertPresenterButton button = m_Buttons[index];

					view.SetButtonEnabled(index, button.Enabled);
					view.SetButtonVisible(index, button.Visible);
					view.SetButtonSelected(index, button.UseDismissColor);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Methods

		/// <summary>
		/// Set the message of the presenter and show it.
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="buttons"></param>
		public void Show(string message, params GenericAlertPresenterButton[] buttons)
		{
			if (buttons == null)
				throw new ArgumentNullException("buttons");

			Show(message, 0, buttons);
		}

		/// <summary>
		/// Set the message of the presenter and show it for the given time.
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="timeout">Time in milliseconds to show the popup</param>
		/// <param name="buttons"></param>
		public void Show(string message, long timeout, params GenericAlertPresenterButton[] buttons)
		{
			if (buttons == null)
				throw new ArgumentNullException("buttons");

			m_RefreshSection.Enter();

			try
			{
				m_Message = message;

				m_Buttons.Clear();
				m_Buttons.AddRange(buttons);

				m_Timed = timeout > 0;
				if (m_Timed)
					m_CloseTimer.Reset(timeout);
				else
					m_CloseTimer.Stop();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
			
			Refresh();
			ShowView(true);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IGenericAlertView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IGenericAlertView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			ShowView(false);

			GenericAlertPresenterButton button;

			m_RefreshSection.Enter();

			try
			{
				if (!m_Buttons.TryElementAt(eventArgs.Data, out button))
					return;
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			button.PressCallback(this);
		}

		#endregion
	}
}
