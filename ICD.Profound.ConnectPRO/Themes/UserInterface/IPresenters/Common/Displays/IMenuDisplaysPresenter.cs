using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public delegate void MenuDestinationPressedCallback(
		object sender, ISource routedSource, IDestination destination);

	public interface IMenuDisplaysPresenter : IPresenter<IMenuDisplaysView>
	{
		event MenuDestinationPressedCallback OnDestinationPressed;

		/// <summary>
		/// Sets the source that is currently active for routing.
		/// </summary>
		ISource ActiveSource { get; set; }

		void SetActiveAudioSources(IcdHashSet<ISource> activeAudio);

		void SetRoutedSources(Dictionary<IDestination, ISource> routing);
	}
}
