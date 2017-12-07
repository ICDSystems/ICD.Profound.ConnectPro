using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources
{
	public interface IReferencedSourceSelectPresenter : IPresenter<IReferencedSourceSelectView>
	{
		ISource Source { get; set; }
	}
}
