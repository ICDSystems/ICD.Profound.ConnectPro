using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference
{
	public interface IAtcIncomingCallPresenter : IUiPresenter<IAtcIncomingCallView>
	{
		/// <summary>
		/// Raised when the user answers the call.
		/// </summary>
		event EventHandler<GenericEventArgs<IConferenceDeviceControl>>  OnCallAnswered;
	}
}
