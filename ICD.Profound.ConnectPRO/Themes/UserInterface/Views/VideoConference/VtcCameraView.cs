using System;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcCameraView : AbstractView, IVtcCameraView
	{
		public event EventHandler OnCameraMoveUpButtonPressed;
		public event EventHandler OnCameraMoveLeftButtonPressed;
		public event EventHandler OnCameraMoveRightButtonPressed;
		public event EventHandler OnCameraMoveDownButtonPressed;
		public event EventHandler OnCameraZoomInButtonPressed;
		public event EventHandler OnCameraZoomOutButtonPressed;
		public event EventHandler OnCameraButtonReleased;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcCameraView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
