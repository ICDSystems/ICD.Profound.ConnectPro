using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views
{
	public abstract class AbstractTouchDisplayView : AbstractView, ITouchDisplayView
	{
		private readonly ConnectProTheme m_Theme;

		public ConnectProTheme Theme { get { return m_Theme; } }

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractTouchDisplayView(ISigInputOutput panel, ConnectProTheme theme) : base(panel)
		{
			m_Theme = theme;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractTouchDisplayView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index) : base(panel, parent, index)
		{
			m_Theme = theme;
		}

		#endregion
	}
}
