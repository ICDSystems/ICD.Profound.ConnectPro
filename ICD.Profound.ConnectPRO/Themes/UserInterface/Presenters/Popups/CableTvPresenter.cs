using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Connect.TvPresets;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups
{
	public sealed class CableTvPresenter : AbstractPopupPresenter<ICableTvView>, ICableTvPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly Dictionary<int, Station> m_Stations;

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
		public CableTvPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Stations = new Dictionary<int, Station>();
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
				// Set station list and dictionary
				m_Stations.Clear();
				ushort i = 0;

				IcdConsole.PrintLine(eConsoleColor.Magenta, "Station count: {0}", Theme.TvPresets.Count);

				foreach (Station station in Theme.TvPresets)
				{
					m_Stations[i] = station;
					view.SetStationButtonIcon(i, station.Url);
					i++;
				}
				view.SetStationListSize(i);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICableTvView view)
		{
			base.Subscribe(view);

			view.OnChannelButtonPressed += ViewOnChannelButtonPressed;

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

			view.OnChannelButtonPressed -= ViewOnChannelButtonPressed;

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

		private void ViewOnChannelButtonPressed(object sender, UShortEventArgs uShortEventArgs)
		{
			if (Control == null)
				return;

			string channel = m_Stations[uShortEventArgs.Data].Channel;
			Control.SetChannel(channel);
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
	}
}
