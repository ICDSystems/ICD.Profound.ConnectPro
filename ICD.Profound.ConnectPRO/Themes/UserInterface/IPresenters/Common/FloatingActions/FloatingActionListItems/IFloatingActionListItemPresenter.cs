using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions.FloatingActionListItems;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions.FloatingActionListItems
{
	public interface IFloatingActionListItemPresenter
	{
		event EventHandler<BoolEventArgs> OnIsAvaliableChanged;

		bool IsAvailable { get; }
		string Label { get; }
		string Icon { get; }

		// True if this action is currently showing - used for active state of shortcut button
		bool GetActive();

		/// <summary>
		/// This method will get called when the button is pressed - also called by the FloatingActionListButtonPresenter if this is the only icon
		/// </summary>
		void HandleButtonPressed();
	}

	public interface IFloatingActionListItemPresenter<T> : IFloatingActionListItemPresenter, IUiPresenter<T> where T : IFloatingActionListItemView
	{
		
	}
}
