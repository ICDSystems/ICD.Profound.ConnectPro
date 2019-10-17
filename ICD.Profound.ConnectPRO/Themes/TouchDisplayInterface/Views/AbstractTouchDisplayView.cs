using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views
{
	public abstract class AbstractTouchDisplayView : AbstractView, ITouchDisplayView
	{
		public ConnectProTheme Theme { get; }

		#region Constructors

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractTouchDisplayView(ISigInputOutput panel, ConnectProTheme theme) 
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
		protected AbstractTouchDisplayView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent,
			ushort index) : base(panel, parent, index)
		{
			Theme = theme;
		}

		#endregion
	}
}