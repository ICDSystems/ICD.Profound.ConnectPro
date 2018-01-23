using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcReferencedHangupPresenter : IPresenter<IVtcReferencedHangupView>
	{
		/// <summary>
		/// Sets the conference source for this presenter.
		/// </summary>
		/// <value></value>
		IConferenceSource ConferenceSource { get; set; }
	}
}
