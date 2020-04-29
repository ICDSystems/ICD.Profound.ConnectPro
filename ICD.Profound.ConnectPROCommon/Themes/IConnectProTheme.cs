using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Themes;

namespace ICD.Profound.ConnectPROCommon.Themes
{
	public delegate void StartRoomCombine(IConnectProTheme sender, int openCount, int closeCount);

	public interface IConnectProTheme : ITheme
	{
		event EventHandler OnCueBackgroundChanged;

		/// <summary>
		/// Raised when starting to combine rooms.
		/// </summary>
		event StartRoomCombine OnStartRoomCombine;

		/// <summary>
		/// Raised when ending combining rooms.
		/// </summary>
		event EventHandler<GenericEventArgs<Exception>> OnEndRoomCombine;

		/// <summary>
		/// Gets/sets the configured relative or absolute path to the logo image for the splash screen.
		/// </summary>
		string Logo { get; set; }

		/// <summary>
		/// Gets/sets the CUE background mode.
		/// </summary>
		eCueBackgroundMode CueBackground { get; set; }

		bool CueMotion { get; set; }

		/// <summary>
		/// Gets the date formatting rules.
		/// </summary>
		ConnectProDateFormatting DateFormatting { get; }
	}
}