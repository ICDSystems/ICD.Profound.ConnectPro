using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups
{
	public interface ICableTvPresenter : IPopupPresenter<ICableTvView>
	{
		/// <summary>
		/// Gets/sets the tv tuner control that this preseter controls.
		/// </summary>
		ITvTunerControl Control { get; set; }
	}
}
