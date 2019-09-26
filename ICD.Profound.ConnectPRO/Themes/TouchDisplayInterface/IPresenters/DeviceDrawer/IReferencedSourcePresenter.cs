using System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer
{
	public interface IReferencedSourcePresenter : ITouchDisplayPresenter<IReferencedSourceView>
	{
		event EventHandler OnSourcePressed;

		ISource Source { get; set; }
	}
}