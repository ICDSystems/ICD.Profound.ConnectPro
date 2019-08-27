using ICD.Common.Utils.EventArguments;
using System;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface ICameraActiveView : IUiView
	{
		#region Events

		/// <summary>
		/// Raised when a camera button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnCameraButtonPressed;

		/// <summary>
		/// Raised when a tab button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnTabButtonPressed;

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

		/// <summary>
		/// Sets the selected state of a tab button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetTabSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the visibility of the tab button
		/// </summary>
		/// <param name="visible"></param>
		void SetTabVisibility(bool visible);
	}
}
