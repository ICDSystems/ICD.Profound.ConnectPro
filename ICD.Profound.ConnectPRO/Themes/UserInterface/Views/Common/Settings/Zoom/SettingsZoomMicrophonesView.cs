using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Zoom
{
	[ViewBinding(typeof(ISettingsZoomMicrophonesView))]
    public sealed partial class SettingsZoomMicrophonesView : AbstractUiView, ISettingsZoomMicrophonesView
    {
	    /// <summary>
	    /// Raised when the user presses a microphone button from the list.
	    /// </summary>
	    public event EventHandler<UShortEventArgs> OnMicrophoneButtonPressed;

	    /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="theme"></param>
		public SettingsZoomMicrophonesView(ISigInputOutput panel, ConnectProTheme theme) 
            : base(panel, theme)
        {
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        public override void Dispose()
        {
			OnMicrophoneButtonPressed = null;

            base.Dispose();
        }

        #region Methods

	    /// <summary>
	    /// Sets the labels for the microphone dynamic button list.
	    /// </summary>
	    /// <param name="labels"></param>
	    public void SetMicrophoneLabels(IEnumerable<string> labels)
	    {
			m_MicrophoneButtonList.SetItemLabels(labels.ToArray());
	    }

	    /// <summary>
	    /// Sets the selection state of the microphone button at the specified index.
	    /// </summary>
	    /// <param name="index"></param>
	    /// <param name="selected"></param>
	    public void SetMicrophoneButtonSelected(ushort index, bool selected)
	    {
			m_MicrophoneButtonList.SetItemSelected(index, selected);
	    }

        #endregion

        #region Control Callbacks

        /// <summary>
        /// Subscribes to the view controls.
        /// </summary>
        protected override void SubscribeControls()
        {
            base.SubscribeControls();

            m_MicrophoneButtonList.OnButtonClicked += MicrophoneButtonListOnButtonClicked;
        }

        /// <summary>
        /// Unsubscribes from the view controls.
        /// </summary>
        protected override void UnsubscribeControls()
        {
            base.UnsubscribeControls();

            m_MicrophoneButtonList.OnButtonClicked -= MicrophoneButtonListOnButtonClicked;
        }

        /// <summary>
        /// Called when the user clicks a camera device button from the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MicrophoneButtonListOnButtonClicked(object sender, UShortEventArgs e)
        {
            OnMicrophoneButtonPressed.Raise(this, new UShortEventArgs(e.Data));
        }

        #endregion
    }
}
