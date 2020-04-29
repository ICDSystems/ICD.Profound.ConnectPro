using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews
{
	public interface IVolumeView : ITouchDisplayView
	{
		event EventHandler OnVolumeUpButtonPressed;
		event EventHandler OnVolumeDownButtonPressed;
		event EventHandler OnVolumeButtonReleased;
		event EventHandler OnMuteButtonPressed;
		event EventHandler<UShortEventArgs> OnVolumeGaugePressed;

		void SetMuteButtonVisible(bool visible);

		void SetMuted(bool muted);

		void SetVolumePercentage(float volume);
	}
}