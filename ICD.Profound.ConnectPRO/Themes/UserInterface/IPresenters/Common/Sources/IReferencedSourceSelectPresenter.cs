using System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources
{
	public interface IReferencedSourceSelectPresenter : IUiPresenter<IReferencedSourceSelectView>
	{
		/// <summary>
		/// Raised when the user presses the presenter.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Gets/sets the source for this presenter.
		/// </summary>
		ISource Source { get; set; }

		/// <summary>
		/// Gets/sets the selected state for this presenter.
		/// </summary>
		bool Selected { get; set; }

		/// <summary>
		/// Gets/sets the routed state for this presenter.
		/// </summary>
		eSourceState SourceState { get; set; }
	}
}
