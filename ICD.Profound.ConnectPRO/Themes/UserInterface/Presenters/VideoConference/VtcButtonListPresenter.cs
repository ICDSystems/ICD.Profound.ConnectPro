using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	[PresenterBinding(typeof(IVtcButtonListPresenter))]
	public sealed class VtcButtonListPresenter : AbstractUiPresenter<IVtcButtonListView>, IVtcButtonListPresenter
	{
		public const ushort INDEX_ACTIVE_CALLS = 0;
		private const ushort INDEX_SHARE = 1;
		private const ushort INDEX_DTMF = 2;

		private static readonly Dictionary<ushort, string> s_ButtonLabels =
			new Dictionary<ushort, string>
			{
				{INDEX_ACTIVE_CALLS, "Active Calls"},
				{INDEX_SHARE, "Share"},
				{INDEX_DTMF, "Touchtones"}
			};

		private static readonly Dictionary<ushort, Type> s_NavTypes =
			new Dictionary<ushort, Type>
			{
				{INDEX_ACTIVE_CALLS, typeof(IVtcActiveCallsPresenter)},
				{INDEX_SHARE, typeof(IVtcSharePresenter)},
				{INDEX_DTMF, typeof(IVtcDtmfPresenter)}
			};

		private readonly Dictionary<ushort, IUiPresenter> m_NavPages; 
		private readonly SafeCriticalSection m_RefreshSection;

		private IUiPresenter m_Visible;

		public IUiPresenter Visible
		{
			get { return m_Visible; }
			set
			{
				if (value == m_Visible)
					return;

				m_Visible = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcButtonListPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_NavPages = new Dictionary<ushort, IUiPresenter>();
			foreach (KeyValuePair<ushort, Type> kvp in s_NavTypes)
				m_NavPages.Add(kvp.Key, nav.LazyLoadPresenter(kvp.Value) as IUiPresenter);

			SubscribePages();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			UnsubscribePages();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcButtonListView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<string> labels = s_ButtonLabels.OrderValuesByKey();
				view.SetButtonLabels(labels);

				foreach (KeyValuePair<ushort, IUiPresenter> kvp in m_NavPages)
				{
					view.SetButtonVisible(kvp.Key, true);
					view.SetButtonEnabled(kvp.Key, true);
					view.SetButtonSelected(kvp.Key, kvp.Value.IsViewVisible);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void ShowMenu(ushort menu)
		{
			m_NavPages[menu].ShowView(true);
		}

		#region Page Callbacks

		/// <summary>
		/// Subscribe to the child page events.
		/// </summary>
		private void SubscribePages()
		{
			foreach (IUiPresenter presenter in m_NavPages.Values)
				presenter.OnViewVisibilityChanged += PresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the child page events.
		/// </summary>
		private void UnsubscribePages()
		{
			foreach (IUiPresenter presenter in m_NavPages.Values)
				presenter.OnViewVisibilityChanged -= PresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when a child page visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void PresenterOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			if (boolEventArgs.Data)
				Visible = sender as IUiPresenter;
			else if (Visible == sender)
				Visible = null;

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcButtonListView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcButtonListView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		private void ViewOnButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			ShowMenu(eventArgs.Data);
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
			{
				ShowMenu(INDEX_ACTIVE_CALLS);
			}
			else
			{
				foreach (IUiPresenter presenter in m_NavPages.Values)
					presenter.ShowView(false);
			}
		}

		#endregion
	}
}
