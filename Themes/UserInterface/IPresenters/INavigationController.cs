using System;
using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters
{
	/// <summary>
	/// INavigationController provides a way for presenters to access each other.
	/// </summary>
	public interface INavigationController : IDisposable
	{
		/// <summary>
		/// Sets the room for the presenters to represent.
		/// </summary>
		/// <param name="room"></param>
		void SetRoom(IConnectProRoom room);

		/// <summary>
		/// Instantiates or returns an existing presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IPresenter LazyLoadPresenter(Type type);

		/// <summary>
		/// Instantiates a new presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IPresenter GetNewPresenter(Type type);
	}

	/// <summary>
	/// Extension methods for INavigationControllers.
	/// </summary>
	public static class NavigationControllerExtensions
	{
		/// <summary>
		/// Instantiates or returns an existing presenter of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T LazyLoadPresenter<T>(this INavigationController extends)
			where T : class, IPresenter
		{
			IPresenter presenter = extends.LazyLoadPresenter(typeof(T));
			T output = presenter as T;

			if (output == null)
				throw new InvalidCastException(string.Format("Failed to cast {0} to {1}", presenter, typeof(T).Name));

			return output;
		}

		/// <summary>
		/// Instantiates a new presenter of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetNewPresenter<T>(this INavigationController extends)
			where T : class, IPresenter
		{
			IPresenter presenter = extends.GetNewPresenter(typeof(T));
			T output = presenter as T;

			if (output == null)
				throw new InvalidCastException(string.Format("Failed to cast {0} to {1}", presenter, typeof(T).Name));

			return output;
		}

		/// <summary>
		/// Generates new presenters if count exceeds the cache size, returns the given number of items from the cache.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="cache"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetNewPresenters<T>(this INavigationController extends, List<T> cache, int count)
			where T : class, IPresenter
		{
			for (int index = 0; index < count; index++)
			{
				if (index >= cache.Count)
					cache.Add(extends.GetNewPresenter<T>());
				yield return cache[index];
			}
		}

		/// <summary>
		/// Shows the presenter of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static T NavigateTo<T>(this INavigationController extends)
			where T : class, IPresenter
		{
			T output = extends.LazyLoadPresenter<T>();
			output.ShowView(true);
			return output;
		}
	}
}
