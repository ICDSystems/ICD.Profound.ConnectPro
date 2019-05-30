using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv
{
	public interface ICableTvPresenter : IPopupPresenter<ICableTvView>, IContextualControlPresenter
	{
		/// <summary>
		/// Gets/sets the tv tuner control that this preseter controls.
		/// </summary>
		ITvTunerControl Control { get; set; }
	}
}
