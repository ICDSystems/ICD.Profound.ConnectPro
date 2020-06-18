using System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.TouchFreeConferencing
{
	public interface IReferencedSettingsTouchFreePresenter : IUiPresenter<IReferencedSettingsTouchFreeView>
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
	}
}