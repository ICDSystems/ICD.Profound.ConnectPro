using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras
{
	public interface ICameraActiveView : IUiView
	{
		#region Events

		/// <summary>
		/// Raised when a camera button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnCameraButtonPressed;

		#endregion
		
		/// <summary>
		/// Sets the camera selection list labels.
		/// </summary>
		/// <param name="labels"></param>
		void SetCameraLabels(IEnumerable<string> labels);

		/// <summary>
		/// Sets the selection state of the camera button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetCameraSelected(ushort index, bool selected);
	}
}
