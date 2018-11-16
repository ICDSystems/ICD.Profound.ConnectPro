using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	/// <summary>
	/// Simple binding class to ensure component views always have a parent and an index.
	/// </summary>
	public abstract class AbstractComponentView : AbstractUiView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractComponentView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}
	}
}
