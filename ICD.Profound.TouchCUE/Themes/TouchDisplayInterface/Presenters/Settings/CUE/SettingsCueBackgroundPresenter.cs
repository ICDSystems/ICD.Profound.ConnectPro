using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.SettingsTree.CUE;
using ICD.Profound.ConnectPROCommon.Themes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings.CUE;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings.CUE.Modes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.CUE;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Settings.CUE
{
	[PresenterBinding(typeof(ISettingsCueBackgroundPresenter))]
	public sealed class SettingsCueBackgroundPresenter : AbstractSettingsNodeBasePresenter<ISettingsCueBackgroundView, BackgroundSettingsLeaf>,
	                                                     ISettingsCueBackgroundPresenter
	{
		private static readonly BiDictionary<eCueBackgroundMode, Type> s_BackgroundModeToPresenterType =
			new BiDictionary<eCueBackgroundMode, Type>
			{
				{eCueBackgroundMode.Neutral, typeof(ISettingsCueBackgroundStaticPresenter)},
				{eCueBackgroundMode.Monthly, typeof(ISettingsCueBackgroundSeasonalPresenter)}
			};

		private readonly SafeCriticalSection m_RefreshSection;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsCueBackgroundPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			Subscribe(theme);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(Theme);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsCueBackgroundView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				bool motion = Node.BackgroundMotion;

				view.SetSeasonalButtonSelection(Node.BackgroundMode == eCueBackgroundMode.Monthly);
				view.SetStaticButtonSelected(Node.BackgroundMode == eCueBackgroundMode.Neutral);
				view.SetToggleSelected(motion);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private void ToggleEnabled()
		{
			if (Node == null)
				return;

			Node.BackgroundMotion = !Node.BackgroundMotion;
			RefreshIfVisible();
		}

		private void NavigateTo(eCueBackgroundMode mode)
		{
			if (Node == null)
				return;

			Node.BackgroundMode = mode;

			// Hide the old subpage
			foreach (Type type in s_BackgroundModeToPresenterType.Where(kvp => kvp.Key != mode).Select(kvp => kvp.Value))
				Navigation.LazyLoadPresenter(type).ShowView(false);

			// Show the new subpage
			Navigation.LazyLoadPresenter(s_BackgroundModeToPresenterType.GetValue(mode)).ShowView(true);
			
			RefreshIfVisible();
		}

		#endregion

		#region Theme Callbacks

		/// <summary>
		/// Subscribe to the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Subscribe(TouchCueTheme theme)
		{
			theme.OnCueBackgroundChanged += ThemeOnCueBackgroundChanged;
		}

		/// <summary>
		/// Unsubscribe from the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Unsubscribe(TouchCueTheme theme)
		{
			theme.OnCueBackgroundChanged -= ThemeOnCueBackgroundChanged;
		}

		/// <summary>
		/// Called when the CUE background mode changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ThemeOnCueBackgroundChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsCueBackgroundView view)
		{
			base.Subscribe(view);

			view.OnSeasonalButtonPressed += ViewOnSeasonalButtonPressed;
			view.OnStaticButtonPressed += ViewOnStaticButtonPressed;
			view.OnTogglePressed += ViewOnTogglePressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsCueBackgroundView view)
		{
			base.Unsubscribe(view);

			view.OnSeasonalButtonPressed -= ViewOnSeasonalButtonPressed;
			view.OnStaticButtonPressed -= ViewOnStaticButtonPressed;
			view.OnTogglePressed -= ViewOnTogglePressed;
		}

		private void ViewOnTogglePressed(object sender, EventArgs eventArgs)
		{
			ToggleEnabled();
		}

		private void ViewOnStaticButtonPressed(object sender, EventArgs eventArgs)
		{
			NavigateTo(eCueBackgroundMode.Neutral);
		}

		private void ViewOnSeasonalButtonPressed(object sender, EventArgs eventArgs)
		{
			NavigateTo(eCueBackgroundMode.Monthly);
		}

		/// <summary>
		/// Called when the view visibility is about to change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			if (args.Data)
				return;

			// Hide all of the subpages before this presenter is hidden
			foreach (Type presenterType in s_BackgroundModeToPresenterType.Values)
				Navigation.LazyLoadPresenter(presenterType).ShowView(false);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// When the view becomes visible show the child presenter
			if (args.Data)
				NavigateTo(Node.BackgroundMode);
		}

		#endregion
	}
}
