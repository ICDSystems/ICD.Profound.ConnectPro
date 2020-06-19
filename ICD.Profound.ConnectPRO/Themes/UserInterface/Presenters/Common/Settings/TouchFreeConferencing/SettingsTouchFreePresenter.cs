using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPROCommon.SettingsTree.TouchFreeConferencing;
using SourcePressedCallback = ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources.SourcePressedCallback;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.TouchFreeConferencing
{
	[PresenterBinding(typeof(ISettingsTouchFreePresenter))]
	public sealed class SettingsTouchFreePresenter : AbstractSettingsNodeBasePresenter<ISettingsTouchFreeView, TouchFreeSettingsLeaf>, ISettingsTouchFreePresenter
	{
		/// <summary>
		/// Raised when the user presses a source.
		/// </summary>
		public event SourcePressedCallback OnSourcePressed;

		private readonly ReferencedSettingsTouchFreePresenterFactory m_ChildrenFactory;

		private readonly Repeater m_Repeater;
		private readonly SafeCriticalSection m_RefreshSection;

		private ISource[] m_Sources;
		private ISource m_SelectedSource;
		private ushort m_DisplayCount;
		private const long BEFORE_REPEAT = 500;
		private const long SECONDS_BETWEEN_REPEAT = 250;

		/// <summary>
		/// Gets/sets the source that is currently selected for routing.
		/// </summary>
		public ISource SelectedSource
		{
			get { return m_SelectedSource; }
			set
			{
				if (value == m_SelectedSource)
					return;

				m_SelectedSource = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsTouchFreePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Repeater = new Repeater();
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSettingsTouchFreePresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsTouchFreeView view)
		{
		}

		#region Private Methods

		private void ToggleEnableZeroTouch()
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				//Todo
				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			Refresh();
			
		}

		private void IncrementCountDownTimer(int seconds)
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;
				//Todo- increment seconds

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
				//Todo- Decrement seconds

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

			SourcePressedCallback handler = OnSourcePressed;
			if (handler != null)
				handler(this, child.Source);
		}

		#endregion

		#region Node Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(TouchFreeSettingsLeaf node)
		{
			
		}

		/// <summary>
		/// Unsubscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(TouchFreeSettingsLeaf node)
		{

		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(ISettingsTouchFreeView node)
		{

		}

		/// <summary>
		/// Unsubscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(ISettingsTouchFreeView node)
		{

		}

		#endregion
	}
}