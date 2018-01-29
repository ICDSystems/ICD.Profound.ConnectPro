using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Hangup;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Hangup
{
	public sealed class VtcReferencedHangupPresenter : AbstractComponentPresenter<IVtcReferencedHangupView>,
	                                                   IVtcReferencedHangupPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private IConferenceSource m_Source;

		/// <summary>
		/// Sets the conference source for this presenter.
		/// </summary>
		/// <value></value>
		public IConferenceSource ConferenceSource
		{
			get { return m_Source; }
			set
			{
				if (value == m_Source)
					return;

				Unsubscribe(m_Source);
				m_Source = value;
				Subscribe(m_Source);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcReferencedHangupPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcReferencedHangupView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetContactName(m_Source == null ? null : m_Source.Name);
				view.SetContactNumber(m_Source == null ? null : m_Source.Number);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Source Callbacks

		/// <summary>
		/// Subscribe to the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Subscribe(IConferenceSource source)
		{
			if (source == null)
				return;

			source.OnNameChanged += SourceOnNameChanged;
			source.OnNumberChanged += SourceOnNumberChanged;
		}

		/// <summary>
		/// Unsubscribe from the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Unsubscribe(IConferenceSource source)
		{
			if (source == null)
				return;

			source.OnNameChanged -= SourceOnNameChanged;
			source.OnNumberChanged -= SourceOnNumberChanged;
		}

		private void SourceOnNumberChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		private void SourceOnNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IVtcReferencedHangupView view)
		{
			base.Subscribe(view);

			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
		}

		protected override void Unsubscribe(IVtcReferencedHangupView view)
		{
			base.Unsubscribe(view);

			view.OnHangupButtonPressed -= ViewOnHangupButtonPressed;
		}

		private void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_Source != null)
				m_Source.Hangup();
		}

		#endregion
	}
}
