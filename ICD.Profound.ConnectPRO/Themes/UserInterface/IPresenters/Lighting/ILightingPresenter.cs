using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Lighting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Lighting
{
	interface ILightingPresenter : IUiPresenter<ILightingView>
	{
		event EventHandler<BoolEventArgs> OnAvalabilityChanged;
	}
}
