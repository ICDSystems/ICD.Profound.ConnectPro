using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views
{
	/// <summary>
	///     Simple binding class to ensure component views always have a parent and an index.
	/// </summary>
	public abstract class AbstractTouchDisplayComponentView : AbstractTouchDisplayView
	{
		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractTouchDisplayComponentView(ISigInputOutput panel, TouchCueTheme theme, IVtProParent parent,
			ushort index)
			: base(panel, theme, parent, index)
		{
		}
	}
}