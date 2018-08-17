using System.Collections.Generic;
using ICD.Common.Utils.Collections;

namespace ICD.Profound.ConnectPRO.Themes
{
	public interface IConnectProUserInterfaceFactory
	{
		/// <summary>
		/// Disposes the instantiated UIs.
		/// </summary>
		void Clear();

		/// <summary>
		/// Instantiates the user interfaces for the originators in the core.
		/// </summary>
		/// <returns></returns>
		void BuildUserInterfaces();

		/// <summary>
		/// Returns the user interfaces for the originators in the core.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IUserInterface> GetUserInterfaces();
	}
}
