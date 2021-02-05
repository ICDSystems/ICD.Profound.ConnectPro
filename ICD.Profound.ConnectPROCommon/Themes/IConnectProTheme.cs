using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Partitioning.Partitions;
using ICD.Connect.Sources.TvTuner.TvPresets;
using ICD.Connect.Themes;
using ICD.Profound.ConnectPROCommon.WebConferencing;

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
		/// Gets the tv presets.
		/// </summary>
		ITvPresets TvPresets { get; }

		/// <summary>
		/// Gets the web conferencing instructions.
		/// </summary>
		WebConferencingInstructions WebConferencingInstructions { get; }

		/// <summary>
		/// Gets/sets the CUE background mode.
		/// </summary>
		eCueBackgroundMode CueBackground { get; set; }

		bool CueMotion { get; set; }

		/// <summary>
		/// Gets the date formatting rules.
		/// </summary>
		ConnectProDateFormatting DateFormatting { get; }

		/// <summary>
		/// Gets/sets the absolute path to the configured logo image for the splash screen.
		/// </summary>
		string LogoAbsolutePath { get; }

		/// <summary>
		/// Opens and closes the given partitions to update the combine spaces.
		/// </summary>
		/// <param name="open"></param>
		/// <param name="close"></param>
		void CombineRooms(IEnumerable<IPartition> open, IEnumerable<IPartition> close);
	}
}