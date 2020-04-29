using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.SettingsTree;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Settings
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
		public SettingsPromptPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
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
				string title = Node == null ? null : Node.Name;

				view.SetHelpText(help);
				view.SetImage(image);
				view.SetTitle(title);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}
	}
}
