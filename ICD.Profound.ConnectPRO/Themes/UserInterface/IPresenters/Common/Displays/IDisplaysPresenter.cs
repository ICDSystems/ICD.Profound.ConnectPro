using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public delegate void MenuDestinationPressedCallback(
		object sender, ISource routedSource, IDestination destination);

	public interface IDisplaysPresenter : IUiPresenter
	{
		event MenuDestinationPressedCallback OnDestinationPressed;

		/// <summary>
		/// Sets the source that is currently active for routing.
		/// </summary>
		ISource SelectedSource { get; set; }

		void SetRouting(IDictionary<IDestination, IcdHashSet<ISource>> routing, IcdHashSet<ISource> activeAudio);
	}
}
