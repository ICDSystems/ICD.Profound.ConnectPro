using System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources
{
	public interface IReferencedSourceSelectPresenter : IPresenter<IReferencedSourceSelectView>
	{
		event EventHandler OnPressed;

		ISource Source { get; set; }

		bool Selected { get; set; }

		bool Routed { get; set; }
	}
}
