using System;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public interface IUnifyBarButtonUi : IDisposable
	{
		/// <summary>
		/// Raised when the button visibility changes.
		/// </summary>
		event EventHandler<BoolEventArgs> OnVisibleChanged;

		/// <summary>
		/// Gets the room.
		/// </summary>
		ICommercialRoom Room { get; }

		/// <summary>
		/// Gets the button.
		/// </summary>
		UnifyBarMainButton Button { get; }

		/// <summary>
		/// Gets the button visibility.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Sets the room.
		/// </summary>
		/// <param name="room"></param>
		void SetRoom([CanBeNull] ICommercialRoom room);

		/// <summary>
		/// Sets the button.
		/// </summary>
		/// <param name="button"></param>
		void SetButton([CanBeNull] UnifyBarMainButton button);
	}
}
