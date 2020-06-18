using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPROCommon.SettingsTree.TouchFreeConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.TouchFreeConferencing
{
	[PresenterBinding(typeof(ISettingsTouchFreePresenter))]
	public sealed class SettingsTouchFreePresenter : AbstractSettingsNodeBasePresenter<ISettingsTouchFreeView, TouchFreeSettingsLeaf>, ISettingsTouchFreePresenter
	{
		private readonly Repeater m_Repeater;
		private readonly SafeCriticalSection m_RefreshSection;

		private const long BEFORE_REPEAT = 500;
		private const long SECONDS_BETWEEN_REPEAT = 250;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsTouchFreePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Repeater = new Repeater();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsTouchFreeView view)
		{
		}

		#region Methods

		private void ToggleEnableZeroTouch()
		{
			
		}

		private void IncrementCountDownTimer(int seconds)
		{
			
		}

		private void DeCrementCountDownTimer(int seconds)
		{
			
		}

		#endregion

		#region Node Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(TouchFreeSettingsLeaf node)
		{
			
		}

		/// <summary>
		/// Unsubscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(TouchFreeSettingsLeaf node)
		{

		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(ISettingsTouchFreeView node)
		{

		}

		/// <summary>
		/// Unsubscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(ISettingsTouchFreeView node)
		{

		}

		#endregion
	}
}