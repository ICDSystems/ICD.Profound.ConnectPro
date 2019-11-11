using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras
{
	public interface ICameraLayoutView : IUiView
	{
		#region Events

		/// <summary>
		/// Raised when a size layout configuration button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnLayoutSizeButtonPressed;

		/// <summary>
		/// Raised when a style layout configuration button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnLayoutStyleButtonPressed;

		/// <summary>
		/// Raised when a share layout configuration button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnLayoutShareButtonPressed;

		/// <summary>
		/// Raised when a self-view layout configuration button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnLayoutSelfViewButtonPressed;

		/// <summary>
		/// Raised when a position layout configuration button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnLayoutPositionButtonPressed;

		#endregion

		#region Methods

		/// <summary>
		/// Sets the selected state of a button for the size layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetLayoutSizeControlButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the selected state of a button for the style layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetLayoutStyleControlButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the selected state of a button for the share layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetLayoutShareControlButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the selected state of a button for the self-view layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetLayoutSelfViewControlButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the selected state of a button for the position layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetLayoutPositionControlButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the enabled state of the size layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutSizeListEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the style layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutStyleListEnable(bool enabled);

		/// <summary>
		/// Sets the enabled state of the share layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutShareListEnable(bool enabled);

		/// <summary>
		/// Sets the enabled state of the self-view layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutSelfViewListEnable(bool enabled);

		/// <summary>
		/// Sets the enabled state of the position layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutPositionListEnable(bool enabled);

		#endregion
	}
}
