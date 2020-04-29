using System.Collections.Generic;

namespace ICD.Profound.ConnectPROCommon.SettingsTree
{
	public interface ISettingsNode : ISettingsNodeBase
	{
		/// <summary>
		/// The path to the image graphic shown when this node is active.
		/// </summary>
		string Image { get; }

		/// <summary>
		/// The help text shown when this node is active.
		/// </summary>
		string Prompt { get; }

		/// <summary>
		/// Gets the child nodes.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ISettingsNodeBase> GetChildren();
	}
}