using System.Collections.Generic;
using ICD.Connect.Conferencing.Participants;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls
{
	public interface IVtcActiveCallsPresenter : IUiPresenter<IVtcActiveCallsView>
	{
		/// <summary>
		/// Returns the current active sources.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ITraditionalParticipant> GetSources();

		/// <summary>
		/// Hangs up all of the active sources.
		/// </summary>
		void HangupAll();
	}
}
