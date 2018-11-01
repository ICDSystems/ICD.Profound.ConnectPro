using ICD.Connect.Conferencing.Participants;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls
{
	public interface IVtcReferencedActiveCallsPresenter : IPresenter<IVtcReferencedActiveCallsView>
	{
		/// <summary>
		/// Sets the conference source for this presenter.
		/// </summary>
		/// <value></value>
		ITraditionalParticipant Participant { get; set; }
	}
}
