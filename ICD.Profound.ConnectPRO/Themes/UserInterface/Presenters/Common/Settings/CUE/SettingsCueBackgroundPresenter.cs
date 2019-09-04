using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.CUE;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.CUE.Modes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.CUE;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.CUE
{
	[PresenterBinding(typeof(ISettingsCueBackgroundPresenter))]
	public sealed class SettingsCueBackgroundPresenter : AbstractUiPresenter<ISettingsCueBackgroundView>,
	                                                     ISettingsCueBackgroundPresenter
	{
		private const ushort INDEX_STATIC = 0;
		private const ushort INDEX_SEASONAL = 1;

		private static readonly BiDictionary<ushort, Type> s_IndexToPresenterType =
			new BiDictionary<ushort, Type>
			{
				{INDEX_STATIC, typeof(ISettingsCueBackgroundStaticPresenter)},
				{INDEX_SEASONAL, typeof(ISettingsCueBackgroundSeasonalPresenter)}
			};

		private static readonly BiDictionary<ushort, eCueBackgroundMode> s_IndexToBackgroundMode =
			new BiDictionary<ushort, eCueBackgroundMode>
			{
				{INDEX_STATIC, eCueBackgroundMode.Neutral},
				{INDEX_SEASONAL, eCueBackgroundMode.Monthly}
			};

		private readonly SafeCriticalSection m_RefreshSection;

		private ushort m_Index;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsCueBackgroundPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			Subscribe(theme);

			ushort activeIndex = s_IndexToBackgroundMode.GetKey(theme.CueBackground);
			NavigateTo(activeIndex);
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
				bool enabled = GetEnabledState();

				view.SetSeasonalButtonSelection(m_Index == INDEX_SEASONAL);
				view.SetStaticButtonSelected(m_Index == INDEX_STATIC);
				view.SetToggleSelected(enabled);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private bool GetEnabledState()
		{
			return s_IndexToBackgroundMode.GetValue(m_Index) == Theme.CueBackground;
		}

		private void ToggleEnabled()
		{
			bool enabled = GetEnabledState();

			// Enable is easy
			if (!enabled)
			{
				Theme.CueBackground = s_IndexToBackgroundMode.GetValue(m_Index);
				return;
			}

			// Disable reverts to neutral mode UNLESS we're already on neutral mode,
			// in which case we go to seasonal mode
			Theme.CueBackground =
				m_Index == INDEX_STATIC
					? eCueBackgroundMode.Monthly
					: eCueBackgroundMode.Neutral;
		}

		private void NavigateTo(ushort index)
		{
			m_Index = index;

			// Hide the old subpage
			foreach (Type type in s_IndexToPresenterType.Where(kvp => kvp.Key != m_Index).Select(kvp => kvp.Value))
				Navigation.LazyLoadPresenter(type).ShowView(false);

			// Show the new subpage
			Navigation.LazyLoadPresenter(s_IndexToPresenterType.GetValue(m_Index));

			RefreshIfVisible();
		}

		#endregion

		#region Theme Callbacks

		/// <summary>
		/// Subscribe to the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Subscribe(ConnectProTheme theme)
		{
			theme.OnCueBackgroundChanged += ThemeOnCueBackgroundChanged;
		}

		/// <summary>
		/// Unsubscribe from the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Unsubscribe(ConnectProTheme theme)
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
			NavigateTo(INDEX_STATIC);
		}

		private void ViewOnSeasonalButtonPressed(object sender, EventArgs eventArgs)
		{
			NavigateTo(INDEX_SEASONAL);
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
			foreach (Type presenterType in s_IndexToPresenterType.Values)
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
				NavigateTo(m_Index);
		}

		#endregion
	}
}
