using ICD.Connect.Panels.Devices;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views
{
	public sealed class ConnectProTouchDisplayViewFactory : AbstractViewFactory, ITouchDisplayViewFactory
	{
		private readonly TouchCueTheme m_Theme;

		public ConnectProTouchDisplayViewFactory(IPanelDevice panel, TouchCueTheme theme) : base(panel)
		{
			m_Theme = theme;
		}

		protected override T InstantiateView<T>()
		{
			return InstantiateView<T>(Panel, m_Theme);
		}

		protected override T InstantiateView<T>(ISmartObject smartObject, IVtProParent parent, ushort index)
		{
			return InstantiateView<T>(smartObject, m_Theme, parent, index);
		}
	}
}