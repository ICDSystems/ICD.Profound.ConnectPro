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
	}
}
