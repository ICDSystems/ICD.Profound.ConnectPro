using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Headers
{
	public interface IOsdHeaderPresenter : IOsdPresenter
	{
		eTouchFreeFace FaceImage { get; set; }
	}
}
