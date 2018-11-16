using System;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public class WtcJoinByIdPresenter : AbstractWtcPresenter<IWtcJoinByIdView>, IWtcJoinByIdPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly StringBuilder m_Builder;

		public WtcJoinByIdPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_Builder = new StringBuilder();

			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IWtcJoinByIdView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetText(m_Builder.ToString());
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		protected override void Subscribe(IWtcJoinByIdView view)
		{
			base.Subscribe(view);

			view.OnTextEntered += ViewOnOnTextEntered;
			view.OnBackButtonPressed += ViewOnOnBackButtonPressed;
			view.OnClearButtonPressed += ViewOnOnClearButtonPressed;
			view.OnKeypadButtonPressed += ViewOnOnKeypadButtonPressed;
			view.OnJoinMyMeetingButtonPressed += ViewOnOnJoinMyMeetingButtonPressed;
		}
		
		protected override void Unsubscribe(IWtcJoinByIdView view)
		{
			base.Unsubscribe(view);

			view.OnTextEntered -= ViewOnOnTextEntered;
			view.OnBackButtonPressed -= ViewOnOnBackButtonPressed;
			view.OnClearButtonPressed -= ViewOnOnClearButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnOnKeypadButtonPressed;
			view.OnJoinMyMeetingButtonPressed -= ViewOnOnJoinMyMeetingButtonPressed;
		}

		private void ViewOnOnJoinMyMeetingButtonPressed(object sender, EventArgs e)
		{
			if (ActiveConferenceControl == null)
				return;

			ActiveConferenceControl.Dial(new ZoomDialContext { DialString = m_Builder.ToString() });
			RefreshIfVisible();
		}

		private void ViewOnOnKeypadButtonPressed(object sender, CharEventArgs e)
		{
			m_Builder.Append(e.Data);
			RefreshIfVisible();
		}

		private void ViewOnOnClearButtonPressed(object sender, EventArgs e)
		{
			m_Builder.Clear();
			RefreshIfVisible();
		}

		private void ViewOnOnBackButtonPressed(object sender, EventArgs e)
		{
			m_Builder.Remove(m_Builder.Length - 1, 1);
			RefreshIfVisible();
		}

		private void ViewOnOnTextEntered(object sender, StringEventArgs e)
		{
			m_Builder.Clear();
			m_Builder.Append(e.Data);
		}

		#endregion
	}
}