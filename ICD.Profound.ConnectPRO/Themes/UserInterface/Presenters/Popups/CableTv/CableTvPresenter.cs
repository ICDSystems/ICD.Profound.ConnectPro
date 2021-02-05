using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Connect.Sources.TvTuner.TvPresets;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.CableTv
{
	[PresenterBinding(typeof(ICableTvPresenter))]
	public sealed class CableTvPresenter : AbstractPopupPresenter<ICableTvView>, ICableTvPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedCableTvPresenterFactory m_ChildrenFactory;

		private readonly Station[] m_Stations;

		/// <summary>
		/// Gets/sets the tv tuner control that this preseter controls.
		/// </summary>
		public ITvTunerControl Control { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CableTvPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedCableTvPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);

			m_Stations = theme.TvPresets.ToArray();

			m_ChildrenFactory.BuildChildren(m_Stations);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			UnsubscribeChildren();
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ICableTvView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				foreach (IReferencedCableTvPresenter presenter in m_ChildrenFactory)
					presenter.ShowView(true);

				view.ShowSwipeIcons(m_Stations.Length > 6);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the device control for the presenter.
		/// </summary>
		/// <param name="control"></param>
		public void SetControl(IDeviceControl control)
		{
			Control = control as ITvTunerControl;
		}

		/// <summary>
		/// Returns true if the presenter is able to interact with the given device control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public bool SupportsControl(IDeviceControl control)
		{
			return control is ITvTunerControl;
		}

		/// <summary>
		/// Unsubscribes from all of the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IReferencedCableTvPresenter presenter in m_ChildrenFactory)
				Unsubscribe(presenter);
		}

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IReferencedCableTvView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICableTvView view)
		{
			base.Subscribe(view);

			view.OnGuideButtonPressed += ViewOnGuideButtonPressed;
			view.OnExitButtonPressed += ViewOnExitButtonPressed;
			view.OnPowerButtonPressed += ViewOnPowerButtonPressed;

			view.OnNumberButtonPressed += ViewOnNumberButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnEnterButtonPressed += ViewOnEnterButtonPressed;

			view.OnUpButtonPressed += ViewOnUpButtonPressed;
			view.OnDownButtonPressed += ViewOnDownButtonPressed;
			view.OnLeftButtonPressed += ViewOnLeftButtonPressed;
			view.OnRightButtonPressed += ViewOnRightButtonPressed;
			view.OnSelectButtonPressed += ViewOnSelectButtonPressed;

			view.OnChannelUpButtonPressed += ViewOnChannelUpButtonPressed;
			view.OnChannelDownButtonPressed += ViewOnChannelDownButtonPressed;
			view.OnPageUpButtonPressed += ViewOnPageUpButtonPressed;
			view.OnPageDownButtonPressed += ViewOnPageDownButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICableTvView view)
		{
			base.Unsubscribe(view);

			view.OnGuideButtonPressed -= ViewOnGuideButtonPressed;
			view.OnExitButtonPressed -= ViewOnExitButtonPressed;
			view.OnPowerButtonPressed -= ViewOnPowerButtonPressed;

			view.OnNumberButtonPressed -= ViewOnNumberButtonPressed;
			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnEnterButtonPressed -= ViewOnEnterButtonPressed;

			view.OnUpButtonPressed -= ViewOnUpButtonPressed;
			view.OnDownButtonPressed -= ViewOnDownButtonPressed;
			view.OnLeftButtonPressed -= ViewOnLeftButtonPressed;
			view.OnRightButtonPressed -= ViewOnRightButtonPressed;
			view.OnSelectButtonPressed -= ViewOnSelectButtonPressed;

			view.OnChannelUpButtonPressed -= ViewOnChannelUpButtonPressed;
			view.OnChannelDownButtonPressed -= ViewOnChannelDownButtonPressed;
			view.OnPageUpButtonPressed -= ViewOnPageUpButtonPressed;
			view.OnPageDownButtonPressed -= ViewOnPageDownButtonPressed;
		}

		private void ViewOnPowerButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Power();
		}

		private void ViewOnExitButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Return();
		}

		private void ViewOnGuideButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.PopupMenu();
		}

		private void ViewOnNumberButtonPressed(object sender, CharEventArgs eventArgs)
		{
			if (Control != null)
				Control.SendNumber(eventArgs.Data);
		}

		private void ViewOnClearButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Clear();
		}

		private void ViewOnEnterButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Enter();
		}

		private void ViewOnUpButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Up();
		}

		private void ViewOnDownButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Down();
		}

		private void ViewOnLeftButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Left();
		}

		private void ViewOnRightButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Right();
		}

		private void ViewOnSelectButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Select();
		}

		private void ViewOnChannelUpButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.ChannelUp();
		}

		private void ViewOnChannelDownButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.ChannelDown();
		}

		private void ViewOnPageUpButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.PageUp();
		}

		private void ViewOnPageDownButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.PageDown();
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribe to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(IReferencedCableTvPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(IReferencedCableTvPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed -= ChildOnPressed;
		}

		/// <summary>
		/// Called when the user presses the child source.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnPressed(object sender, EventArgs eventArgs)
		{
			IReferencedCableTvPresenter child = sender as IReferencedCableTvPresenter;
			if (child == null)
				return;

			if (Control != null)
				Control.SetChannel(child.Station.Channel);
		}

		#endregion
	}
}
