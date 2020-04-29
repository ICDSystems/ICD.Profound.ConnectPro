using System;
using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters
{
	[PresenterBinding(typeof(IOsdBackgroundPresenter))]
	public sealed class OsdBackgroundPresenter : AbstractOsdPresenter<IOsdBackgroundView>, IOsdBackgroundPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		public OsdBackgroundPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme) : base(nav, views, theme)
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

		private void Subscribe(IConnectProTheme theme)
		{
			theme.OnCueBackgroundChanged += ThemeOnCueBackgroundChanged;
		}

		private void Unsubscribe(IConnectProTheme theme)
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
