using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Cameras
{
	public sealed partial class CamerasView : AbstractView, ICamerasView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public CamerasView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
