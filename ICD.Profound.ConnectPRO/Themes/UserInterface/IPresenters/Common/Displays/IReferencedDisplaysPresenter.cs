using System;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public interface IReferencedDisplaysPresenter : IPresenter<IReferencedDisplaysView>
	{
		event EventHandler OnPressed;

		IDestination Destination { get; set; }

		/// <summary>
		/// Sets the source that is currently active for routing.
		/// </summary>
		ISource ActiveSource { get; set; }

		/// <summary>
		/// Gets/sets the source that is currently routed to the display.
		/// </summary>
		ISource RoutedSource { get; set; }
	}
}
