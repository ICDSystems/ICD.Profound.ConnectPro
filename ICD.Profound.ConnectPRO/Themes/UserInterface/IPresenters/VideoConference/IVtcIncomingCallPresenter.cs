using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using System;
using ICD.Connect.Conferencing.Controls.Dialing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcIncomingCallPresenter : IPresenter<IVtcIncomingCallView>
	{
		/// <summary>
		/// Raised when the user answers the incoming call.
		/// </summary>
		event EventHandler<GenericEventArgs<IConferenceDeviceControl>> OnCallAnswered;
	}
}
