using System.Collections.Generic;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources
{
	public delegate void SourcePressedCallback(object sender, ISource source);

	public interface ISourceSelectPresenter<TView> : IPresenter<TView>
		where TView : ISourceSelectView
	{
		/// <summary>
		/// Raised when the user presses a source.
		/// </summary>
		event SourcePressedCallback OnSourcePressed;

		/// <summary>
		/// Gets/sets the source that is currently selected for routing.
		/// </summary>
		ISource ActiveSource { get; set; }

		void SetRoutedSources(Dictionary<IDestination, ISource> routing);
	}
}
