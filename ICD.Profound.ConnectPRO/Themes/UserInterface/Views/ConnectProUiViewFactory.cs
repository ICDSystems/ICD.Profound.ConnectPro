using ICD.Connect.Panels.Devices;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	/// <summary>
	/// Provides a way for presenters to access their views.
	/// </summary>
	public sealed class ConnectProUiViewFactory : AbstractViewFactory, IUiViewFactory
	{
		private readonly ConnectProTheme m_Theme;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProUiViewFactory(IPanelDevice panel, ConnectProTheme theme)
			: base(panel)
		{
			m_Theme = theme;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Instantiates a new view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected override T InstantiateView<T>()
		{
			return InstantiateView<T>(Panel, m_Theme);
		}

		/// <summary>
		/// Instantiates a view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="smartObject"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override T InstantiateView<T>(ISmartObject smartObject, IVtProParent parent, ushort index)
		{
			return InstantiateView<T>(smartObject, m_Theme, parent, index);
		}

		#endregion
	}
}
