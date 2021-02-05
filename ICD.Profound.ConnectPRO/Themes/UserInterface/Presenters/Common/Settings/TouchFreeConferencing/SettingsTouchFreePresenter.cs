using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPROCommon.SettingsTree.TouchFreeConferencing;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.TouchFreeConferencing
{
	[PresenterBinding(typeof(ISettingsTouchFreePresenter))]
	public sealed class SettingsTouchFreePresenter : AbstractSettingsNodeBasePresenter<ISettingsTouchFreeView, TouchFreeSettingsLeaf>, ISettingsTouchFreePresenter
	{
		private const long BEFORE_REPEAT = 500;
		private const long SECONDS_BETWEEN_REPEAT = 250;

		private readonly ReferencedSettingsTouchFreePresenterFactory m_ChildrenFactory;

		private readonly Repeater m_Repeater;
		private readonly SafeCriticalSection m_RefreshSection;

		private ISource[] m_Sources;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsTouchFreePresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Repeater = new Repeater();
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSettingsTouchFreePresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);

			m_Sources = new ISource[0];
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsTouchFreeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetCountDownSeconds(Node == null ? 0 : Node.CountDownSeconds);
				view.SetTouchFreeToggleSelected(Node != null && Node.TouchFreeEnabled);

				foreach (IReferencedSettingsTouchFreePresenter child in m_ChildrenFactory.BuildChildren(m_Sources))
				{
					child.Selected = Node != null && child.Source == Node.DefaultSource;
					child.ShowView(true);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private void UpdateSources()
		{
			m_Sources = Node == null ? new ISource[0] : Node.GetSources().ToArray();
			RefreshIfVisible();	
		}

		private void ToggleEnableTouchFree()
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				Node.SetTouchFreeEnabled(!Node.TouchFreeEnabled);
				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		private void IncrementCountDownTimer(int seconds)
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				Node.SetCountdownSeconds(Node.CountDownSeconds + seconds);
				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
			
		}

		private void DeCrementCountDownTimer(int seconds)
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				Node.SetCountdownSeconds(Node.CountDownSeconds - seconds);
				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IReferencedSettingsTouchFreeView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#endregion

		#region Child Callbacks

		private void Subscribe(IReferencedSettingsTouchFreePresenter child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnPressed;
		}

		private void Unsubscribe(IReferencedSettingsTouchFreePresenter child)
		{
			if (child == null)
				return;

			child.OnPressed -= ChildOnPressed;
		}

		private void ChildOnPressed(object sender, EventArgs eventArgs)
		{
			IReferencedSettingsTouchFreePresenter child = sender as IReferencedSettingsTouchFreePresenter;
			if (child == null)
				return;

			if (Node == null)
				return;

			ISource source = child.Source;
			ISource defaultSource = source == Node.DefaultSource ? null : source;

			Node.SetDefaultSource(defaultSource);
			Node.SetDirty(true);
		}

		#endregion

		#region Node Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(TouchFreeSettingsLeaf node)
		{
			base.Subscribe(node);

			if (node == null)
				return;

			node.OnCountdownSecondsChanged += NodeOnCountdownSecondsChanged;
			node.OnDefaultSourceChanged += NodeOnDefaultSourceChanged;
			node.OnTouchFreeEnabledChanged += NodeOnTouchFreeEnabledChanged;
			node.OnSourcesChanged += NodeOnSourcesChanged;

			UpdateSources();
		}

		/// <summary>
		/// Unsubscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(TouchFreeSettingsLeaf node)
		{
			base.Unsubscribe(node);

			if (node == null)
				return;

			node.OnCountdownSecondsChanged -= NodeOnCountdownSecondsChanged;
			node.OnDefaultSourceChanged -= NodeOnDefaultSourceChanged;
			node.OnTouchFreeEnabledChanged -= NodeOnTouchFreeEnabledChanged;
			node.OnSourcesChanged -= NodeOnSourcesChanged;

			UpdateSources();
		}

		private void NodeOnTouchFreeEnabledChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		private void NodeOnDefaultSourceChanged(object sender, SourceEventArgs sourceEventArgs)
		{
			RefreshIfVisible();
		}

		private void NodeOnCountdownSecondsChanged(object sender, IntEventArgs intEventArgs)
		{
			RefreshIfVisible();
		}

		private void NodeOnSourcesChanged(object sender, EventArgs eventArgs)
		{
			UpdateSources();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsTouchFreeView view)
		{
			base.Subscribe(view);

			view.OnCountDownTimerDecrementButtonPressed += ViewOnCountDownTimerDecrementButtonPressed;
			view.OnCountDownTimerIncrementButtonPressed += ViewOnCountDownTimerIncrementButtonPressed;
			view.OnIncrementDecrementButtonReleased += ViewOnIncrementDecrementButtonReleased;
			view.OnEnableZeroTouchTogglePressed += ViewOnEnableZeroTouchTogglePressed;
		}

		/// <summary>
		/// Unsubscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsTouchFreeView view)
		{
			base.Unsubscribe(view);

			view.OnCountDownTimerDecrementButtonPressed -= ViewOnCountDownTimerDecrementButtonPressed;
			view.OnCountDownTimerIncrementButtonPressed -= ViewOnCountDownTimerIncrementButtonPressed;
			view.OnIncrementDecrementButtonReleased -= ViewOnIncrementDecrementButtonReleased;
			view.OnEnableZeroTouchTogglePressed -= ViewOnEnableZeroTouchTogglePressed;
		}

		private void ViewOnEnableZeroTouchTogglePressed(object sender, EventArgs eventArgs)
		{
			ToggleEnableTouchFree();
		}

		private void ViewOnIncrementDecrementButtonReleased(object sender, EventArgs eventArgs)
		{
			m_Repeater.Stop();
		}

		private void ViewOnCountDownTimerIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Repeater.Start(b => IncrementCountDownTimer(1), BEFORE_REPEAT, SECONDS_BETWEEN_REPEAT);
		}

		private void ViewOnCountDownTimerDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Repeater.Start(b => DeCrementCountDownTimer(1), BEFORE_REPEAT, SECONDS_BETWEEN_REPEAT);
		}

		#endregion
	}
}