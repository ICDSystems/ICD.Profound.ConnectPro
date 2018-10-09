using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters
{
	public interface IHelloPresenter : IOsdPresenter<IHelloView>
	{
		event EventHandler<BoolEventArgs> OnMainPageViewChanged;

		/// <summary>
		/// Gets whether the "hello" message is the main focus on the OSD
		/// </summary>
		bool MainPageView { get; }
	}
}
