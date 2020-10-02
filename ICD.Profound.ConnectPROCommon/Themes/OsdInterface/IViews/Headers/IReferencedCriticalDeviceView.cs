namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers
{
	public interface IReferencedCriticalDeviceView : IOsdView
	{
		/// <summary>
		/// Sets the name, severity, and error of the offline device.
		/// </summary>
		/// <param name="text"></param>
		void SetSubjectLabel(string text);

		/// <summary>
		/// Sets the device's icon.
		/// </summary>
		void SetSubjectIcon(string icon);
	}
}
