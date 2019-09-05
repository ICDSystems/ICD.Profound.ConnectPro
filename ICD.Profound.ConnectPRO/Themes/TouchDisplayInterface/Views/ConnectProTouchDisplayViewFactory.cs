using System;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views
{
	public sealed class ConnectProTouchDisplayViewFactory : AbstractViewFactory, ITouchDisplayViewFactory
	{
		private readonly ConnectProTheme m_Theme;

		public ConnectProTouchDisplayViewFactory(IPanelDevice panel, ConnectProTheme theme) : base(panel)
		{
			m_Theme = theme;
		}

		protected override T InstantiateView<T>()
		{
			return InstantiateView<T>(Panel, m_Theme);
		}

		protected override T InstantiateView<T>(ISmartObject smartObject, IVtProParent parent, ushort index)
		{
			return InstantiateView<T>(Panel, m_Theme, parent, index);
		}
	}
}
