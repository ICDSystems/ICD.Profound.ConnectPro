using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions
{
	public interface IFloatingActionListView : IUiView
	{
		event EventHandler<IntEventArgs> OnButtonPressed;

		int MaxItems { get; }

		void ShowItem(int index, string label, string icon);

		void HideItem(int index);
	}
}
