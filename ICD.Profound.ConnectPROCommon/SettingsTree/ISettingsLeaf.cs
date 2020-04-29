using System;

namespace ICD.Profound.ConnectPROCommon.SettingsTree
{
	public interface ISettingsLeaf : ISettingsNodeBase
	{
		/// <summary>
		/// Raised when the settings change.
		/// </summary>
		event EventHandler OnSettingsChanged;
	}
}