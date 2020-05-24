using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom
{
	public interface ISettingsZoomSpeakersView : IUiView
    {
        /// <summary>
        /// Raised when the user presses a speaker button from the list.
        /// </summary>
        event EventHandler<UShortEventArgs> OnSpeakerButtonPressed;

        /// <summary>
        /// Sets the labels for the speaker dynamic button list.
        /// </summary>
        /// <param name="labels"></param>
        void SetSpeakerLabels(IEnumerable<string> labels);

        /// <summary>
        /// Sets the selection state of the speaker button at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        void SetSpeakerButtonSelected(ushort index, bool selected);
    }
}
