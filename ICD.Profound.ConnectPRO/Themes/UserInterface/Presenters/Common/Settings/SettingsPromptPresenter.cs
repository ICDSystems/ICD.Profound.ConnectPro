using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;
using ICD.Profound.ConnectPROCommon.SettingsTree;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	[PresenterBinding(typeof(ISettingsPromptPresenter))]
	public sealed class SettingsPromptPresenter : AbstractSettingsNodeBasePresenter<ISettingsPromptView, ISettingsNode>,
	                                              ISettingsPromptPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsPromptPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsPromptView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string help = Node == null ? null : Node.Prompt;
				string image = Node == null ? null : Node.Image;

				view.SetHelpText(help);
				view.SetImage(image);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}
	}
}
