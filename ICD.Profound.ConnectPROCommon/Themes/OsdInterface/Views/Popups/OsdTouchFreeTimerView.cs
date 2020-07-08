using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Popups;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Popups
{
	[ViewBinding(typeof(IOsdTouchFreeTimerView))]
	public sealed partial class OsdTouchFreeTimerView : AbstractOsdView, IOsdTouchFreeTimerView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public OsdTouchFreeTimerView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetTimer(int seconds)
		{
			m_Timer.SetLabelText(seconds.ToString());
		}
	}
}