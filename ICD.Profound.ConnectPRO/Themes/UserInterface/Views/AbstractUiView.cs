using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	/// <summary>
	/// Base class for all of the views.
	/// </summary>
	public abstract class AbstractUiView : AbstractView, IUiView
	{
		private readonly IConnectProTheme m_Theme;

		public IConnectProTheme Theme { get { return m_Theme; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractUiView(ISigInputOutput panel, IConnectProTheme theme)
			: this(panel, theme, null, 0)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractUiView(ISigInputOutput panel, IConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
			m_Theme = theme;
		}
	}
}
