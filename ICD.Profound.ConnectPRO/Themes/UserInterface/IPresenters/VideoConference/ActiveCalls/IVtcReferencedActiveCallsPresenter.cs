using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls
{
	public interface IVtcReferencedActiveCallsPresenter : IUiPresenter<IVtcReferencedActiveCallsView>
	{
		/// <summary>
		/// Sets the conference source for this presenter.
		/// </summary>
		/// <value></value>
		IConferenceSource ConferenceSource { get; set; }
	}
}
