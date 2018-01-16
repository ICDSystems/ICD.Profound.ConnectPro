using System.Collections.Generic;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public delegate void DestinationPressedCallback(
		object sender, IReferencedDisplaysPresenter presenter, IDestination destination);

	public interface IDisplaysPresenter : IPresenter<IDisplaysView>
	{
		/// <summary>
		/// Raised when the user presses one of the displays.
		/// </summary>
		event DestinationPressedCallback OnDestinationPressed;

		/// <summary>
		/// Gets/sets the source that is currently selected for routing.
		/// </summary>
		ISource ActiveSource { get; set; }

		void SetRoutedSources(Dictionary<IDestination, ISource> routing);
	}
}
