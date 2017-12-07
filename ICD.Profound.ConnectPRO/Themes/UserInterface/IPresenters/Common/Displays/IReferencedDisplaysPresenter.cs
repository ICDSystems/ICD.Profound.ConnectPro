using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public interface IReferencedDisplaysPresenter : IPresenter<IReferencedDisplaysView>
	{
		IDestination Destination { get; set; }
	}
}
