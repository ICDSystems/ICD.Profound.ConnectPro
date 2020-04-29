using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Camera
{
	public interface ICameraLayoutView : ITouchDisplayView
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
		/// Raised when the content thumbnail button is pressed.
		/// </summary>
		event EventHandler OnContentThumbnailButtonPressed;

		/// <summary>
		/// Raised when the self-view camera button is pressed.
		/// </summary>
		event EventHandler OnSelfviewCameraButtonPressed;

		/// <summary>
		/// Raised when a position layout configuration button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnThumbnailPositionButtonPressed;

		#endregion

		#region Methods

		/// <summary>
		/// Sets the selected state of a button for the size layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetLayoutSizeButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the selected state of a button for the style layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetLayoutStyleButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the selected state of a button for the share layout control.
		/// </summary>
		/// <param name="selected"></param>
		void SetContentThumbnailButtonSelected(bool selected);

		/// <summary>
		/// Sets the selected state of a button for the self-view layout control.
		/// </summary>
		/// <param name="selected"></param>
		void SetSelfviewCameraButtonSelected(bool selected);

		/// <summary>
		/// Sets the selected state of a button for the position layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetThumbnailPositionButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the visibility of a button for the position layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetThumbnailPositionButtonVisibility(ushort index, bool visible);

		/// <summary>
		/// Sets the enabled state of the size layout controls buttons.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutSizeListEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the style layout buttons.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutStyleListEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Gallery item in the style layout dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutStyleGalleryItemEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Speaker item in the style layout dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutStyleSpeakerItemEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Strip item in the style layout dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutStyleStripItemEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Share All item in the style layout dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		void SetLayoutStyleShareAllItemEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the share toggle.
		/// </summary>
		/// <param name="enabled"></param>
		void SetContentThumbnailButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the self-view toggle.
		/// </summary>
		/// <param name="enabled"></param>
		void SetSelfviewCameraButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the position layout controls buttons.
		/// </summary>
		/// <param name="enabled"></param>
		void SetThumbnailPositionListEnabled(bool enabled);

		#endregion
	}
}
