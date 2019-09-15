using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public interface IMenuRouteSummaryPresenter : IUiPresenter<IMenuRouteSummaryView>
	{
		void SetRouting(IDictionary<IDestinationBase, IcdHashSet<ISource>> routing);
	}
}
