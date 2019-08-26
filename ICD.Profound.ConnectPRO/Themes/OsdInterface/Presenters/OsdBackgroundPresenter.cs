using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using System;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	[PresenterBinding(typeof(IOsdBackgroundPresenter))]
	public sealed class OsdBackgroundPresenter : AbstractOsdPresenter<IOsdBackgroundView>, IOsdBackgroundPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		public OsdBackgroundPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			Subscribe(theme);
		}

		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(Theme);
		}

		protected override void Refresh(IOsdBackgroundView view)
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
