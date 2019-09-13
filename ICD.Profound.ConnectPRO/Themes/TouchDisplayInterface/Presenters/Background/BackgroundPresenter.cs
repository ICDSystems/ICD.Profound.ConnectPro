using System;
using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Background;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Background;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Background
{
	[PresenterBinding(typeof(IBackgroundPresenter))]
	public sealed class BackgroundPresenter : AbstractTouchDisplayPresenter<IBackgroundView>, IBackgroundPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		public BackgroundPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			Subscribe(theme);
		}

		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(Theme);
		}

		protected override void Refresh(IBackgroundView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetBackgroundMode(Theme.CueBackground);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Theme Callbacks

		private void Subscribe(ConnectProTheme theme)
		{
			theme.OnCueBackgroundChanged += ThemeOnCueBackgroundChanged;
		}

		private void Unsubscribe(ConnectProTheme theme)
		{
			theme.OnCueBackgroundChanged -= ThemeOnCueBackgroundChanged;
		}

		private void ThemeOnCueBackgroundChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}