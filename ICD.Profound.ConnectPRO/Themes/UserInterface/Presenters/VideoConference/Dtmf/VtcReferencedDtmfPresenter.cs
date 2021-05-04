﻿using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Participants;
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

		private IParticipant m_Source;
		private bool m_Selected;

		#region Properties

		[CanBeNull]
		public IParticipant Source
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

			Source = null;
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
					m_Source == null
						? null
						: string.IsNullOrEmpty(m_Source.Name)
							  ? m_Source.Number
							  : m_Source.Name;

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
		private void Subscribe(IParticipant source)
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
		private void Unsubscribe(IParticipant source)
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
