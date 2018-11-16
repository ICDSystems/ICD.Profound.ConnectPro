using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters
{
	/// <summary>
	/// INavigationController provides a way for presenters to access each other.
	/// </summary>
	public interface IConnectProNavigationController : INavigationController
	{
		/// <summary>
		/// Sets the room for the presenters to represent.
		/// </summary>
		/// <param name="room"></param>
		void SetRoom(IConnectProRoom room);

		/// <summary>
		/// Instantiates or returns an existing presenter for every presenter that can be assigned to the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IEnumerable<IPresenter> LazyLoadPresenters(Type type);

		/// <summary>
		/// Instantiates or returns an existing presenter for every presenter that can be assigned to the given type.
		/// </summary>
		/// <returns></returns>
		IEnumerable<T> LazyLoadPresenters<T>() where T : IPresenter;
	}
}
