using System;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.DeviceDrawer
{
	public interface IReferencedSourceView : ITouchDisplayView
	{
		event EventHandler OnButtonPressed;

		void SetIcon(string icon);

		void SetNameText(string name);

		void SetDescriptionText(string description);
	}
}