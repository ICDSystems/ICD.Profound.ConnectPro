using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Header
{
	public interface IReferencedHeaderButtonPresenter : ITouchDisplayPresenter<IReferencedHeaderButtonView>
	{
		HeaderButtonModel Model { get; set; }
	}
}