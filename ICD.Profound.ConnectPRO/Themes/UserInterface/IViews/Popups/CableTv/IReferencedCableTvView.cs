using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv
{
	public interface IReferencedCableTvView : IView
	{
		event EventHandler OnIconPressed;

		void SetIconPath(string path);
	}
}
