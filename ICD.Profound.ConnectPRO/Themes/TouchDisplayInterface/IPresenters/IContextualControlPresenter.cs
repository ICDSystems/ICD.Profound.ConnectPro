using ICD.Connect.Devices.Controls;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters
{
	public interface IContextualControlPresenter : ITouchDisplayPresenter
	{
		/// <summary>
		/// Sets the device control for the presenter.
		/// </summary>
		/// <param name="control"></param>
		void SetControl(IDeviceControl control);

		/// <summary>
		/// Returns true if the presenter is able to interact with the given device control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		bool SupportsControl(IDeviceControl control);
	}
}
