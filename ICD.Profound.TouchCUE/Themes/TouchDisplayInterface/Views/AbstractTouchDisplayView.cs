using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views
{
	public abstract class AbstractTouchDisplayView : AbstractView, ITouchDisplayView
	{
		public TouchCueTheme Theme { get; private set; }

		#region Constructors

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractTouchDisplayView(ISigInputOutput panel, TouchCueTheme theme) 
			: this(panel, theme, null, 0)
		{
		}

		/// <summary>
		///     Constructor
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractTouchDisplayView(ISigInputOutput panel, TouchCueTheme theme, IVtProParent parent,
			ushort index) : base(panel, parent, index)
		{
			Theme = theme;
		}

		#endregion
	}
}