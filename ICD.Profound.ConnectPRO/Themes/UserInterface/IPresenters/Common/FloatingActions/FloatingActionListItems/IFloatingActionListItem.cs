using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions.FloatingActionListItems
{
	public interface IFloatingActionListItem: IDisposable
	{
		event EventHandler<BoolEventArgs> OnIsAvaliableChanged;

		event EventHandler<BoolEventArgs> OnIsActiveChanged;

		bool IsAvailable { get; }
		string Label { get; }
		string Icon { get; }

		/// <summary>
		/// True if this action is currently showing - used for active state of shortcut button
		/// </summary>
		/// <returns></returns>
		bool IsActive { get; }

		/// <summary>
		/// This method will get called when the button is pressed - also called by the FloatingActionListButtonPresenter if this is the only icon
		/// </summary>
		void HandleButtonPressed();

		void Subscribe(IConnectProRoom room);

		void Unsubscribe(IConnectProRoom room);

		void SetRoom(IConnectProRoom room);
	}
}
