using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public delegate void MenuDestinationPressedCallback(
		object sender, ISource routedSource, IDestinationBase destination);

	public interface IDisplaysPresenter : IUiPresenter
	{
		/// <summary>
		/// Raised when a display destination is pressed.
		/// </summary>
		event MenuDestinationPressedCallback OnDestinationPressed;

		/// <summary>
		/// Sets the source that is actively selected for routing.
		/// </summary>
		void SetSelectedSource(ISource source);

		/// <summary>
		/// Sets the current routing state for the displays.
		/// </summary>
		/// <param name="routing"></param>
		/// <param name="activeAudio"></param>
		void SetRouting(IDictionary<IDestinationBase, IcdHashSet<ISource>> routing, IcdHashSet<ISource> activeAudio);
	}
}
