using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	/// <summary>
	/// Simple binding class to ensure component views always have a parent and an index.
	/// </summary>
	public abstract class AbstractOsdComponentView : AbstractOsdView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractOsdComponentView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}
	}
}
