using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header
{
	public interface IReferencedHeaderButtonPresenter : ITouchDisplayPresenter<IReferencedHeaderButtonView>
	{
		HeaderButtonModel Model { get; set; }
	}
}