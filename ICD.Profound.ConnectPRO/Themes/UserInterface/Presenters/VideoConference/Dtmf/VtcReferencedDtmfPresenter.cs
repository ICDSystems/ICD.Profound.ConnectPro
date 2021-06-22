using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Dtmf;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Dtmf
{
	[PresenterBinding(typeof(IVtcReferencedDtmfPresenter))]
	public sealed class VtcReferencedDtmfPresenter : AbstractUiComponentPresenter<IVtcReferencedDtmfView>,
	                                                 IVtcReferencedDtmfPresenter
	{
		/// <summary>
		/// Raised when the user presses the contact.
		/// </summary>
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private IConference m_Conference;
		private bool m_Selected;

		#region Properties

		[CanBeNull]
		public IConference Conference
		{
			get { return m_Conference; }
			set
			{
				if (value == m_Conference)
					return;

				Unsubscribe(m_Conference);
				m_Conference = value;
				Subscribe(m_Conference);

				RefreshIfVisible();
			}
		}

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

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcReferencedDtmfPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Conference = null;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcReferencedDtmfView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string name =
					m_Conference == null
						? null
						: m_Conference.Name;

				if (string.IsNullOrEmpty(name))
					name = "Unknown";

				bool selected = m_Selected;

				view.SetLabel(name);
				view.SetSelected(selected);
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
		private void Subscribe(IConference source)
		{
			if (source == null)
				return;

			source.OnNameChanged += SourceOnNameChanged;
		}

		/// <summary>
		/// Unsubscribe from the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Unsubscribe(IConference source)
		{
			if (source == null)
				return;

			source.OnNameChanged -= SourceOnNameChanged;
		}

		private void SourceOnNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcReferencedDtmfView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcReferencedDtmfView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
