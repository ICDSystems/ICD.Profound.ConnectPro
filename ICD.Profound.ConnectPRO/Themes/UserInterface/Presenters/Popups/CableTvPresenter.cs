using System;
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
		private const ushort INDEX_TOP_MENU = 0;
		private const ushort INDEX_POPUP_MENU = 1;
		private const ushort INDEX_RETURN = 2;
		private const ushort INDEX_INFO = 3;
		private const ushort INDEX_EJECT = 4;
		private const ushort INDEX_POWER = 5;

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
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICableTvView view)
		{
			base.Subscribe(view);

			view.OnMenuButtonPressed += ViewOnOnMenuButtonPressed;
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

			view.OnRepeatButtonPressed += ViewOnRepeatButtonPressed;
			view.OnRewindButtonPressed += ViewOnRewindButtonPressed;
			view.OnFastForwardButtonPressed += ViewOnFastForwardButtonPressed;
			view.OnStopButtonPressed += ViewOnStopButtonPressed;
			view.OnPlayButtonPressed += ViewOnPlayButtonPressed;
			view.OnPauseButtonPressed += ViewOnPauseButtonPressed;
			view.OnRecordButtonPressed += ViewOnRecordButtonPressed;

			view.OnRedButtonPressed += ViewOnRedButtonPressed;
			view.OnGreenButtonPressed += ViewOnGreenButtonPressed;
			view.OnBlueButtonPressed += ViewOnBlueButtonPressed;
			view.OnYellowButtonPressed += ViewOnYellowButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICableTvView view)
		{
			base.Unsubscribe(view);

			view.OnMenuButtonPressed -= ViewOnOnMenuButtonPressed;
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

			view.OnRepeatButtonPressed -= ViewOnRepeatButtonPressed;
			view.OnRewindButtonPressed -= ViewOnRewindButtonPressed;
			view.OnFastForwardButtonPressed -= ViewOnFastForwardButtonPressed;
			view.OnStopButtonPressed -= ViewOnStopButtonPressed;
			view.OnPlayButtonPressed -= ViewOnPlayButtonPressed;
			view.OnPauseButtonPressed -= ViewOnPauseButtonPressed;
			view.OnRecordButtonPressed -= ViewOnRecordButtonPressed;

			view.OnRedButtonPressed -= ViewOnRedButtonPressed;
			view.OnGreenButtonPressed -= ViewOnGreenButtonPressed;
			view.OnBlueButtonPressed -= ViewOnBlueButtonPressed;
			view.OnYellowButtonPressed -= ViewOnYellowButtonPressed;
		}

		private void ViewOnOnMenuButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			if (Control == null)
				return;

			switch (eventArgs.Data)
			{
				case INDEX_TOP_MENU:
					Control.TopMenu();
					break;
				
				case INDEX_POPUP_MENU:
					Control.PopupMenu();
					break;
				
				case INDEX_RETURN:
					Control.Return();
					break;
				
				case INDEX_INFO:
					Control.Info();
					break;
				
				case INDEX_EJECT:
					Control.Eject();
					break;
				
				case INDEX_POWER:
					Control.Power();
					break;
			}
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

		private void ViewOnRepeatButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Repeat();
		}

		private void ViewOnRewindButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Rewind();
		}

		private void ViewOnFastForwardButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.FastForward();
		}

		private void ViewOnStopButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Stop();
		}

		private void ViewOnPlayButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Play();
		}

		private void ViewOnPauseButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Pause();
		}

		private void ViewOnRecordButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Record();
		}

		private void ViewOnRedButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Red();
		}

		private void ViewOnGreenButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Green();
		}

		private void ViewOnBlueButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Blue();
		}

		private void ViewOnYellowButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Control != null)
				Control.Yellow();
		}

		#endregion
	}
}
