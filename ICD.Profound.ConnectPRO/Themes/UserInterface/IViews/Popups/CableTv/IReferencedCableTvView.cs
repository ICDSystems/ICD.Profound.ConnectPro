using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv
{
	public interface IReferencedCableTvView : IUiView
	{
		event EventHandler OnIconPressed;

		void SetIconPath(string path);
	}
}
