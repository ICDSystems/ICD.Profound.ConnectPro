using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions.FloatingActionListItems;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions
{
	public interface IFloatingActionListPresenter : IUiPresenter<IFloatingActionListView>
	{
		void SetChildren(IEnumerable<IFloatingActionListItem> items);
	}
}
