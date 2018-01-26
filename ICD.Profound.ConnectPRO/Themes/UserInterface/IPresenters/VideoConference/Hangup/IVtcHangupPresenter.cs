using System.Collections.Generic;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Hangup
{
	public interface IVtcHangupPresenter : IPresenter<IVtcHangupView>
	{
		/// <summary>
		/// Returns the current active sources.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IConferenceSource> GetSources();

		/// <summary>
		/// Hangs up all of the active sources.
		/// </summary>
		void HangupAll();
	}
}
