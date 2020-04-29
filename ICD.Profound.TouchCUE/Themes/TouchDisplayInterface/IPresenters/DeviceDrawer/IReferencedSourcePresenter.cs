using System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer
{
	public interface IReferencedSourcePresenter : ITouchDisplayPresenter<IReferencedSourceView>
	{
		event EventHandler OnSourcePressed;

		ISource Source { get; set; }

		eSourceState SourceState { get; set; }
	}
}