using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom
{
	public interface ISettingsZoomMicrophonesView : IUiView
    {
        /// <summary>
        /// Raised when the user presses a microphone button from the list.
        /// </summary>
        event EventHandler<UShortEventArgs> OnMicrophoneButtonPressed;

        /// <summary>
        /// Sets the labels for the microphone dynamic button list.
        /// </summary>
        /// <param name="labels"></param>
        void SetMicrophoneLabels(IEnumerable<string> labels);

        /// <summary>
        /// Sets the selection state of the microphone button at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        void SetMicrophoneButtonSelected(ushort index, bool selected);
    }
}
