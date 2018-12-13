using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public class WtcButtonListPresenter : AbstractWtcPresenter<IWtcButtonListView>, IWtcButtonListPresenter
	{
		private const ushort INDEX_PARTICIPANTS = 0;
		private const ushort INDEX_SHARE = 1;
		private const ushort INDEX_RECORDING = 2;
		private const ushort INDEX_CALL_OUT = 3;

		private static readonly Dictionary<ushort, string> s_ButtonLabels = new Dictionary<ushort, string>
		{
			{INDEX_PARTICIPANTS, "Participants"}, 
			{INDEX_SHARE, "Share"},
			//{INDEX_RECORDING, "Recording"},
			//{INDEX_CALL_OUT, "Call Out"}
		};

		private static readonly Dictionary<ushort, Type> s_NavTypes = new Dictionary<ushort, Type>
		{
			{INDEX_PARTICIPANTS, typeof (IWtcActiveMeetingPresenter)}, 
			{INDEX_SHARE,  typeof (IWtcSharePresenter)},
			//{INDEX_RECORDING, typeof(IWtcRecordingPresenter)},
			//{INDEX_CALL_OUT, typeof(IWtcCallOutPresenter)}
		};

		private readonly Dictionary<ushort, IPresenter> m_NavPages;
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly IWtcContactsTogglePresenter m_TogglePresenter;

		private IPresenter m_Visible;

		public IPresenter Visible
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

		public WtcButtonListPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_NavPages = new Dictionary<ushort, IPresenter>();
			foreach (KeyValuePair<ushort, Type> kvp in s_NavTypes)
				m_NavPages.Add(kvp.Key, nav.LazyLoadPresenter(kvp.Value));

			SubscribePages();

			m_TogglePresenter = nav.LazyLoadPresenter<IWtcContactsTogglePresenter>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			UnsubscribePages();
		}

		protected override void Refresh(IWtcButtonListView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<string> labels = s_ButtonLabels.OrderValuesByKey();
				view.SetButtonLabels(labels);

				foreach (KeyValuePair<ushort, IPresenter> kvp in m_NavPages)
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

		public void ShowMenu(ushort index)
		{
			m_NavPages[index].ShowView(true);
		}

		#region Page Callbacks

		/// <summary>
		/// Subscribe to the child page events.
		/// </summary>
		private void SubscribePages()
		{
			foreach (IPresenter presenter in m_NavPages.Values)
				presenter.OnViewVisibilityChanged += PresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the child page events.
		/// </summary>
		private void UnsubscribePages()
		{
			foreach (IPresenter presenter in m_NavPages.Values)
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
				Visible = sender as IPresenter;
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
		protected override void Subscribe(IWtcButtonListView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWtcButtonListView view)
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
				ShowMenu(INDEX_PARTICIPANTS);
			}
			else
			{
				foreach (IPresenter presenter in m_NavPages.Values)
					presenter.ShowView(false);
			}

			m_TogglePresenter.ShowView(args.Data);
		}

		#endregion
	}
}