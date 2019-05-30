using ICD.Connect.Devices.Controls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters
{
	public interface IContextualControlPresenter : IUiPresenter
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
