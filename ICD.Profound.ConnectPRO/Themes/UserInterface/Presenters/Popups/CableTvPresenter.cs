using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups
{
	public sealed class CableTvPresenter : AbstractPopupPresenter<ICableTvView>, ICableTvPresenter
	{
		private const ushort INDEX_ABC = 0;
		private const ushort INDEX_CBS = 1;
		private const ushort INDEX_NBC = 2;
		private const ushort INDEX_FOX = 3;
		private const ushort INDEX_CNN = 4;
		private const ushort INDEX_WEATHER = 5;
		private const ushort INDEX_ESPN = 6;
		private const ushort INDEX_NBC_SPORTS = 7;

		private static readonly Dictionary<ushort, string> s_Channels =
			new Dictionary<ushort, string>
			{
				{INDEX_ABC, "806"},
				{INDEX_CBS, "803"},
				{INDEX_NBC, "810"},
				{INDEX_FOX, "805"},
				{INDEX_CNN, "817"},
				{INDEX_WEATHER, "815"},
				{INDEX_ESPN, "850"},
				{INDEX_NBC_SPORTS, "846"},
			};

		/// <summary>
		/// Gets/sets the tv tuner control that this preseter controls.
		/// </summary>
		public ITvTunerControl Control { get; set; }

		/// <summary>
		/// Static constructor.
		/// </summary>
		static CableTvPresenter()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CableTvPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
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

			string channel = s_Channels[uShortEventArgs.Data];
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
				Control.TopMenu();
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
